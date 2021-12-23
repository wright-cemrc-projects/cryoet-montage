#!/usr/bin/python3

import os
import argparse
import subprocess

##############################################################################
## CryoMontage tilt series developed by Jae Yang, Wright Lab Oct. 2021
## Last updated 2021-12-22, Matt Larson, Cryo-EM Research Center
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

'''
The function of this script is to process directories containing movie stacks
of a tilt-series data collection with motion correction.
'''

def list_suffix(directory, extension):
    return (f for f in os.listdir(directory) if f.endswith('.' + extension))

class FrameSet:
    ''' Represent a FrameSet entry of mdoc '''
    Index = 0
    ''' Use a python dictionary to flexibly capture MDoc data '''
    nameVal = {}

    def getString(self):
        ''' get a printable representation '''
        lines = []
        lines.append('[FrameSet = ' + str(self.Index) + ']\n')

        for key in self.nameVal:
            line = key + ' = ' + self.nameVal[key] + '\n'
            lines.append(line)

        # Concatenate and return 
        return ''.join(lines)

class MDoc:
    ''' Represent an mdoc metadata file '''
    nameVal = {}

    ''' Frameset entries '''
    framesets = []

    @staticmethod
    def parse(filename):
        ''' parse a filename to create an MDoc (field = value) '''
        rv = MDoc()

        currentFrameSet = None

        # Parse header or FrameSet information
        with open(filename) as file:
            for line in file.readlines():
                ''' Parse a line '''
                line = line.strip()
                if line.startswith('['):
                    ''' May be the start of a new FrameSet entry '''
                    currentFrameSet = FrameSet()
                    rv.framesets.append(currentFrameSet)
                    
                elif line:
                    ''' Split and get the parts '''
                    parts = line.split(' = ')

                    ''' If there are two parts, we'll store values '''
                    if len(parts) > 1:
                        ''' Get the field and value '''
                        field = parts[0]
                        value = parts[1]

                        if currentFrameSet:
                            currentFrameSet.nameVal[field] = value
                        else:
                            rv.nameVal[field] = value
        return rv
                

    def getString(self):
        ''' get a printable representation '''
        lines = []
        
        for key in self.nameVal:
            line = key + ' = ' + self.nameVal[key] + '\n'
            lines.append(line)

        lines.append('\n')
        for fs in self.framesets:
            lines.append(fs.getString())
            lines.append('\n')

        # Concatenate and return 
        return ''.join(lines)

# Motion correction options describing how to run the processing.
class MotionOptions:
    program = 'motionCor2'
    rotGain = None
    flipGain = None
    throwFrames = None

def createMDoc(imageIn, imageOut):
    ''' Read and write update SerialEM metadata (mdoc) '''
    mdocInput = imageIn + '.mdoc'
    mdocOutput = imageOut + '.mdoc'

    mdocText = MDoc.parse(mdocInput)
    with open(mdocOutput, 'w') as file:
        file.write(mdocText.getString())

def motionCorProcess(infile, outfile, pixelSize, motionOptions, gain = None):
    ''' Run motionCor2 if needed for a frame '''
    # Example: MotionCor2 -InTiff ${f} -OutMrc ${f}.cor.mrc -Patch 5 5 -PixSize 4.603 -Gpu 0 1 2 3 
    motionCor = 'motioncor2'
    inputType = "-InTiff" # inputType = "-InTif"

    if (os.path.exists(outfile)):
        print(outfile + ' exists: skipping motionCor')
    else:
        print(outfile + ' will be created by motionCor...')

        args = [motionCor, 
            inputType, infile, 
            "-OutMrc", outfile, 
            "-Patch", "5 5", 
            "-Iter", "10", 
            "-Tol", "0.5", 
            "-PixSize", str(pixelSize)]

        if gain:
            args.append("-Gain")
            args.append(gain)
        
        if motionOptions.rotGain:
            args.append("-RotGain")
            args.append(motionOptions.rotGain)
        
        if motionOptions.flipGain:
            args.append("-FlipGain")
            args.append(motionOptions.flipGain)

        if motionOptions.throwFrames:
            args.append("-Throw")
            args.append(motionOptions.throwFrames)
        

        subprocess.call(args)
    createMDoc(infile, outfile)

def alignFramesProcess(infile, outfile):
    ''' Run alignFrames if needed for a frame '''
    # Example: alignframes -in ${f} -out ${f}.cor.mrc
    motionCor = 'alignframes'

    # TODO: should also consider using -frames or -StartingEndingFrames (two integers)
    #  We may need to exclude the 1st frame, and this may be only way to do so.

    if (os.path.exists(outfile)):
        print(outfile + ' exists: skipping alignframes')
    else:
        print(outfile + ' will be created by alignframes...')
        args = [motionCor, 
            '-in', infile, 
            '-ou', outfile ]

        subprocess.call(args)
    createMDoc(infile, outfile)

def applyMotionCor(tiltDirectory, outputDirectory, pixelSize, motionOptions, gain):
    ''' For each stack in .tif, apply motion correction to produce .mrc '''
    # Process each individual .tif stack with motionCorProcess
    files = list_suffix(tiltDirectory, "tif")
    for f in files:
        relativeInPath = os.path.join(tiltDirectory, f)
        # Note: careful with extension, _ will disrupt subsequent sorting.
        outfile = f[:-len('.tif')] + '.mc.mrc'
        relativeOutPath = os.path.join(outputDirectory, outfile)
        if (motionOptions.program == 'motioncor2'):
            motionCorProcess(relativeInPath, relativeOutPath, pixelSize, motionOptions, gain)
        elif (motionOptions.program == 'alignframes'):
            alignFramesProcess(relativeInPath, relativeOutPath)
        else :
            print('applyMotionCor: undefined alignment program ' + motionOptions.program)
            exit(1)

def alterHeader(stackFile, tiltAxisAngle, binning):
    ''' Add a header text line describing the tilt axis angle '''
    # Example: alterheader AlignedStack_fcor.st -ti "Tilt axis angle = 86.0, binning = 1"
    header = 'Tilt axis angle = {:3.2f}, binning = {:1d}'.format(tiltAxisAngle, int(binning))
    command = 'alterheader'
    args = [ command,
        stackFile,
        '-ti', header
    ]
    subprocess.call(args)
    print("Added header information: " + header)

def getAngle( fn ):
    try:
        name, num = fn.rsplit('_',1)  # split at the rightmost `_`
        num = num.split('.')[0]
        return int(num)
    except ValueError: # no _ in there
        return fn, None

def getMDoc(tiltDirectory):
    ''' Find the first possible mdoc file and return it's mdoc '''
    files = list_suffix(tiltDirectory, "tif")
    for f in files:
        relativeInPath = os.path.join(tiltDirectory, f)
        mdocFile = relativeInPath + '.mdoc'
        if (os.path.exists(mdocFile)):
            print('Extracting parameters from : ' + mdocFile)
            return MDoc.parse(mdocFile)
    return None

def processBatch(tiltDirectory, outputDirectory, motionOptions):
    ''' Given a parent directory, for each child directory process the tilt series '''
    # Process each subdirectory of tiltDirectory as an independent tilt-series
    for childDir in os.listdir(tiltDirectory):
        relativePath = os.path.join(tiltDirectory, childDir)
        if (os.path.isdir(relativePath)):
            outputChild = os.path.join(outputDirectory, childDir)
            processTiltDirectory(relativePath, outputChild, motionOptions)

def processTiltDirectory(relativePath, outputChild, motionOptions):
    ''' Given a tilt directory, process the tilt series '''
    if not os.path.exists(outputChild):
        os.makedirs(outputChild)

    # The 'mdoc' metadata files can provide all the details, if they are available.
    frame_1_mdoc = getMDoc(relativePath)
    pixelSize = 1.0
    gain = None
    if (frame_1_mdoc and len(frame_1_mdoc.framesets) > 0):
        frameDesc = frame_1_mdoc.framesets[0]
        if 'PixelSpacing' in frameDesc.nameVal:
            pixelSize = float(frameDesc.nameVal['PixelSpacing'])
        if 'GainReference' in frameDesc.nameVal:
            gainFile = os.path.join(relativePath, frameDesc.nameVal['GainReference'])
            if (os.path.exists(gainFile)):
                gain = gainFile

    # 2. Motion Corrections 
    applyMotionCor(relativePath, outputChild, pixelSize, motionOptions, gain)

def main():
    # 1. Provide a command-line arguments
    parser = argparse.ArgumentParser(description='Prepare tilt-series data for use')
    parser.add_argument('--batchDirectory', help='parent directory containing multiple tilt stacks', required=False, default=None)
    parser.add_argument('--tiltDirectory', help='directory containing tilt stack', required=False, default=None)
    parser.add_argument('--outputDirectory', help='directory to deposit results', required=True)
    parser.add_argument('--motion', help='options [alignframes|motioncor2], default=motioncor2', default='motioncor2')
    parser.add_argument('--rotGain', help='optional value for MotionCor2, -RotGain 0,1,2,3 how to rotate gain', default=None)
    parser.add_argument('--flipGain', help='optional value for MotionCor2, -FlipGain 0,1,2 how to flip gain', default=None)
    parser.add_argument('--throwFrames', help='optional value for MotionCor2, -Throw XX starting frames away', default=None)
    args = parser.parse_args()

    if (args.motion == 'alignframes' or args.motion == 'motioncor2'):
        ''' ok '''
    else:
        print('--motion options [alignframes|motioncor2]')
        exit(1)

    motionOptions = MotionOptions()
    motionOptions.program = args.motion
    motionOptions.rotGain = args.rotGain
    motionOptions.flipGain = args.flipGain
    motionOptions.throwFrames = args.throwFrames

    if args.batchDirectory :
        processBatch(args.batchDirectory, args.outputDirectory, motionOptions)
    elif args.tiltDirectory :
        processTiltDirectory(args.tiltDirectory, args.outputDirectory, motionOptions)
    else:
        print("required to provide either --batchDirectory or --tiltDirectory")
        exit(1)

if __name__ == "__main__":
    main()