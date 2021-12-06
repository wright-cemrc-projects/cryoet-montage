#!/usr/bin/python3

description_text = 'SplitTomogram: split the tiled images from a tilt-series into a individual images and stack.' 

##############################################################################
## Adapted from `split.sh` to Python scripting
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
import os
import shutil
import fnmatch
import subprocess

def createTiltList(output_directory, basename, subtilt, startTilt, endTilt, stepTilt, tiltFile):
    ''' Create a userlist.txt file defining the files/tilts in the stack '''
    processing_directory = os.path.join(output_directory, basename + '_Processing/')
    f = open(tiltFile, 'w')
    for tilt in range(startTilt, endTilt+1, stepTilt):
        output_name = os.path.join(output_directory, basename + '_subtilt_' + str(subtilt) + '_' + str(tilt) + '.0.mrc')
        f.write(output_name + '\n')
        f.write('0\n')
    f.close()

def createRawTilt(startTilt, endTilt, stepTilt, rawTiltFile):
    ''' Create user.rawtlt '''
    f = open(rawTiltFile, 'w')
    for tilt in range(startTilt, endTilt+1, stepTilt):
        f.write(tilt + '\n')
    f.close()

def copySubtilt(basename, tilt, subtilt, input_directory, output_name):
    ''' Split out the files of an individual tilt '''
    expected_name = basename + '_' + subtilt + '_*_' + str(tilt) + '.0.*.mrc'
    # copy any of the matching tilts where filenames include the basename and tilt angle.
    patterns = [ expected_name ]
    if tilt == 0:
        #  Provide an alternative name for the 0 tilt can be -0.0 or 0.0
        patterns.append( basename + '_' + subtilt + '_*_-' + str(tilt) + '.0.*.mrc' )
        
    for filename in os.listdir(input_directory):
        for pattern in patterns:
            if (fnmatch.fnmatch(filename, expected_name)):
                # store original frame in the stitching directory with expected filename.
                shutil.copyfile(filename, output_name)

def main():
    # 1. Provide commandline options
    parser = argparse.ArgumentParser(description=description_text)

    ## Parameters
    parser.add_argument('--input', help='path to the data collection directory', required=True, default=None)
    parser.add_argument('--output', help='path to a location to write results', required=True, default=None)
    parser.add_argument('--basename', help='define a common basename for the images', required=True, default=None)
    parser.add_argument('--period', help='define number of subtilts', type=int, required=True, default=None)
    parser.add_argument('--first_subtilt', help='define first subtilt index', type=int, required=True, default=1)
    parser.add_argument('--last_subtilt', help='define last subtilt index', type=int, required=True, default=9)
    parser.add_argument('--starting_angle', help='define the minimal bounds of the tilt range, ex. -60', type=int, required=True, default=None)
    parser.add_argument('--ending_angle', help='define the maximal bounds of the tilt range, ex. 60', type=int, required=True, default=None)
    parser.add_argument('--tilt_increment', help='define the increment of the tilt, ex 3', type=int, required=True, default=None)
    args = parser.parse_args()

    ## Split the tilt files for each subtilt into a separate directory, and stack.
    for subtilt in range(args.first_subtilt, args.last_subtilt, args.period):
        outputTiltDir = os.path.join(args.output, 'subTilt_' + str(subtilt))
        os.makedirs(outputTiltDir)
        for tilt in range(args.starting_angle, args.ending_angle+1, args.tilt_increment):
            output_name = os.path.join(outputTiltDir, args.basename + '_subtilt_' + str(subtilt) + '_' + str(tilt) + '.0.mrc')
            copySubtilt(args.basename, tilt, subtilt, args.input, output_name)
        
        rawTiltTxt = os.path.join(outputTiltDir, 'rawtlt.txt')
        createRawTilt(args.starting_angle, args.ending_angle, args.tilt_increment, rawTiltTxt)

        tiltListTxt = os.path.join(outputTiltDir, 'tiltList.txt')
        createTiltList(outputTiltDir, args.basename, subtilt, args.starting_angle, args.ending_angle, args.tilt_increment, tiltListTxt)

        # After copying the subtilts, should be renamed $basename_subtilt_tilt.0.mrc
        finalStackFile = os.path.join(outputTiltDir, args.basename + '_subtilt_' + str(subtilt) + '_' + str(tilt) + '_AliSB.st')

        # Generate usertilt.txt
        subprocess.run('newstack', '-tilt', rawTiltTxt, '-fileinlist', tiltListTxt, '-ou', finalStackFile)


