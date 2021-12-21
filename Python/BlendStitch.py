#!/usr/bin/python3

description_text = 'BlendStitch: blend and stitch tiled images from tilt-series into a larger image and stack.' 

##############################################################################
## Adapted from `blendstitching.sh` to Python scripting
## CryoMontage tilt series developed by Jae Yang, Wright Lab Oct. 2021
## Last updated 2021-11-29, Matt Larson, Cryo-EM Research Center
##
## MIT License
## 
## Copyright (c) 2021 Jae Yang, Matt Larson
## 
## Permission is hereby granted, free of charge, to any person obtaining a copy
## of this software and associated documentation files (the "Software"), to deal
## in the Software without restriction, including without limitation the rights
## to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
## copies of the Software, and to permit persons to whom the Software is
## furnished to do so, subject to the following conditions:
## 
## The above copyright notice and this permission notice shall be included in all
## copies or substantial portions of the Software.
## 
## THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
## IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
## FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
## AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
## LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
## OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
## SOFTWARE.
###############################################################################

import argparse
import shutil
import fnmatch
import os
import subprocess

class StitchPattern():
    ''' defines a stiching pattern from image shifts '''
    camera_x = 5760
    camera_y = 4092
    overlap_x = 0.20
    overlap_y = 0.15
    tile_x = 3
    tile_y = 3

    def writePieceFile(self, filename):
        ''' write out a formatted .pl pattern for IMOD '''
        offset_x = self.camera_x * (1.0 - self.overlap_x)
        offset_y = self.camera_y * (1.0 - self.overlap_y)
        f = open(filename, 'w')
        for c_tile_y in range(0, self.tile_y) :
            ''' zig zag, when y is odd run in opposite direction on x '''
            x_items = range(1, self.tile_x+1)
            if (c_tile_y % 2) != 0:
                x_items = range(self.tile_x, 0, -1)
            for c_tile_x in x_items:
                x = (c_tile_x-1) * offset_x
                y = c_tile_y * offset_y
                z = 0
                f.write(' %8d %8d %6d\n' % (x, y, z))
        f.close()

def stitching(minAngle, maxAngle, stepAngle, input_directory, output_directory, basename, stitchPattern):
    ''' Process each individual tilt with newstack and blendmont and store in Output/Tilt_${i}/${Basename}_${i}'''
    for tilt in range(minAngle, maxAngle+1, stepAngle):
        # make individual folders for each tilt to store original frames and stitched tiles per tilt
        processing_directory = os.path.join(output_directory, basename + '_Processing/')
        stitching_directory = os.path.join(processing_directory, 'Tilt_'+str(tilt))
        if (not os.path.isdir(stitching_directory)):
            os.makedirs(stitching_directory)

        expected_name = basename + '_*_' + str(tilt) + '.0.*.mrc'
        # copy any of the matching tilts where filenames include the basename and tilt angle.
        patterns = [ expected_name ]
        if tilt == 0:
            #  Provide an alternative name for the 0 tilt can be -0.0 or 0.0
            patterns.append( basename + '_*_-' + str(tilt) + '.0.*.mrc' )
            
        for filename in os.listdir(input_directory):
            for pattern in patterns:
                if (fnmatch.fnmatch(filename, pattern)):
                    # store original frame in the stitching directory with expected filename.
                    src = os.path.join(input_directory, filename)
                    dest = os.path.join(stitching_directory, filename)
                    if os.path.exists(dest):
                        print(dest + ' exists, skipping')
                    else:
                        shutil.copyfile(src, dest)
            
        # call newstack on all the files of the tilt directory to make $Basename_$i.st
        # create a -fileinlist to provide the list of all the tiles in the tilt directory.
        tileFileList = os.path.join(stitching_directory, 'tileList.txt')
        createTileList(stitching_directory, tileFileList)
        newstack_output = os.path.join(stitching_directory, basename + '_' + str(tilt) + '.st')
        if os.path.exists(newstack_output):
            print(newstack_output + ' exists, skipping')
        else:
            subprocess.run(['newstack', '-fileinlist', tileFileList, newstack_output])

        # pl file describes pixel distances to stitch tiles together
        blendmont_pl_input = os.path.join(stitching_directory, basename + str(tilt) + '.pl')
        stitchPattern.writePieceFile(blendmont_pl_input)

        # call blendmont with options for ...
        # -plin is a file with piece coordinates
        # -roo is root name for edge function and .ecd files
        # -very is to support large displacements (very sloppy!)
        blendmont_root_name = os.path.join(stitching_directory, basename + '_' + str(tilt))
        filename = basename + '_' + str(tilt) + '_blend.st'
        blendmont_output = os.path.join(stitching_directory, filename)
        
        if os.path.exists(blendmont_output):
            print(blendmont_output + ' exists, skipping')
        else:
            subprocess.run(['blendmont', '-imin', newstack_output, '-imout', blendmont_output, '-plin',  blendmont_pl_input, '-roo', blendmont_root_name, '-very'])
        
        replica_name = os.path.join(processing_directory, filename)
        if os.path.exists(replica_name):  
            print(replica_name + ' exists, skipping')
        else:
            shutil.copyfile(blendmont_output, replica_name)

def createTileList(tile_directory, tileFile):
    ''' Create a userlist.txt file defining the files/tilts in the stack '''
    # Calculate number of files, should be the first line of this userlist.txt
    # Get list of *.mrc in the tile_directory
    files = []
    for file in sorted(os.listdir(tile_directory)):
        if file.endswith(".mrc"):
            files.append(os.path.join(tile_directory, file))
    f = open(tileFile, 'w')
    f.write(str(len(files)) + '\n')
    for file in files:
        f.write(file + '\n')
        f.write('0\n')
    f.close()

def createTiltList(output_directory, basename, startTilt, endTilt, stepTilt, tiltFile):
    ''' Create a userlist.txt file defining the files/tilts in the stack '''
    processing_directory = os.path.join(output_directory, basename + '_Processing/')
    tiltCount = (endTilt - startTilt) / stepTilt + 1
    f = open(tiltFile, 'w')
    f.write(str(tiltCount) + '\n')
    for tilt in range(startTilt, endTilt+1, stepTilt):
        stitching_directory = os.path.join(processing_directory, 'Tilt_'+str(tilt))
        blendmont_output = os.path.join(stitching_directory, basename + '_' + str(tilt) + '_blend.st')
        f.write(blendmont_output + '\n')
        f.write('0\n')
    f.close()

def createRawTilt(startTilt, endTilt, stepTilt, rawTiltFile):
    ''' Create user.rawtlt '''
    f = open(rawTiltFile, 'w')
    for tilt in range(startTilt, endTilt+1, stepTilt):
        f.write(str(tilt) + '\n')
    f.close()

def checkDependencies():
    ''' return true if finds the required IMOD programs '''
    newstack_location = shutil.which('newstack')
    blendmont_location = shutil.which('blendmont')
    return newstack_location and blendmont_location

def restricted_float(x):
    try:
        x = float(x)
    except ValueError:
        raise argparse.ArgumentTypeError("%r not a floating-point literal" % (x,))

    if x < 0.0 or x > 1.0:
        raise argparse.ArgumentTypeError("%r not in range [0.0, 1.0]"%(x,))
    return x

def main():
    # 1. Provide commandline options
    parser = argparse.ArgumentParser(description=description_text)

    ## Parameters
    parser.add_argument('--input', help='path to the data collection directory', required=True, default=None)
    parser.add_argument('--output', help='path to a location to write results', required=True, default=None)
    parser.add_argument('--basename', help='define a common basename for the images', required=True, default=None)
    parser.add_argument('--starting_angle', help='define the minimal bounds of the tilt range, ex. -60', type=int, required=True, default=None)
    parser.add_argument('--ending_angle', help='define the maximal bounds of the tilt range, ex. 60', type=int, required=True, default=None)
    parser.add_argument('--tilt_increment', help='define the increment of the tilt, ex 3', type=int, required=True, default=None)
    ## Parameters describing blend parameters
    parser.add_argument('--camera_x', help='define camera width in pixel dimensions (default 5760)', type=int, required=False, default=5760)
    parser.add_argument('--camera_y', help='define camera height in pixel dimensions (default 4092)', type=int, required=False, default=4092)
    parser.add_argument('--overlap_x', help='define a percent overlap where 15 percent would be 0.15 (default 0.15)', type=restricted_float, required=False, default=0.15)
    parser.add_argument('--overlap_y', help='define a percent overlap where 10 percent would be 0.10 (default 0.10)', type=restricted_float, required=False, default=0.10)
    parser.add_argument('--tile_x', help='define the number of tiles in the x dimension (default 3)', type=int, required=False, default=3)
    parser.add_argument('--tile_y', help='define the number of tiles in the y dimension (default 3)', type=int, required=False, default=3)
    args = parser.parse_args()

    # Check that expected software are in the PATH
    if checkDependencies():
        ''' Dependencies found '''
    else:
        print ('Missing IMOD dependencies in PATH, please install IMOD 4.11.6 and make sure binaries are in your PATH.')
        exit(1)

    # The pattern defines how the imaging areas overlap, important for blending.
    pattern = StitchPattern() 
    pattern.camera_x = args.camera_x
    pattern.camera_y = args.camera_y
    pattern.overlap_x = args.overlap_x
    pattern.overlap_y = args.overlap_y
    pattern.tile_x = args.tile_x
    pattern.tile_y = args.tile_y

    # Run the main 'Stitching' function to create individual sticked images called *_blend.st and stack.
    stitching(args.starting_angle, args.ending_angle, args.tilt_increment, args.input, args.output, args.basename, pattern)

    # Create the rawtlt and tiltlist.txt
    rawTiltTxt = os.path.join(args.output, 'tilt.rawtlt')
    createRawTilt(args.starting_angle, args.ending_angle, args.tilt_increment, rawTiltTxt)
    tiltListTxt = os.path.join(args.output, 'tiltList.txt')
    createTiltList(args.output, args.basename, args.starting_angle, args.ending_angle, args.tilt_increment, tiltListTxt)

    # Output different binned stacks.
    finalStackPrefix = os.path.join(args.output, args.basename + 'AliSB')
    finalStackFile = finalStackPrefix + '.st'
    finalStackFileBin2 = finalStackPrefix + '_bin2.st'
    finalStackFileBin4 = finalStackPrefix + '_bin4.st'

    # generate the stitched nxn unbinned tilt series, 2x binned, and 4x binned tilt series.
    if os.path.exists(finalStackFile):
        print(finalStackFile + ' exists, skipping')
    else:
        subprocess.run(['newstack', '-tilt', rawTiltTxt, '-fileinlist', tiltListTxt, '-ou', finalStackFile])
    if os.path.exists(finalStackFileBin2):
        print(finalStackFileBin2 + ' exists, skipping')
    else:
        subprocess.run(['newstack', '-shrink', '2.0', finalStackFile, finalStackFileBin2])
    if os.path.exists(finalStackFileBin4):
        print(finalStackFileBin4 + ' exists, skipping')
    else:
        subprocess.run(['newstack', '-shrink', '4.0', finalStackFile, finalStackFileBin4])

if __name__ == "__main__":
    main()