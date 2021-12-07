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
## Updated 12/6/2021
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
import re

def getSubtiltFilename(basename, subtilt, tilt):
    ''' helper function for consistent naming '''
    return basename + '_subtilt_' + str(subtilt) + '_' + str(tilt) + '.0.mrc'

def createTiltList(output_directory, basename, subtilt, startTilt, endTilt, stepTilt, tiltFile):
    ''' Create a userlist.txt file defining the files/tilts in the stack '''
    # Calculate number of files, should be the first line of this userlist.txt
    tiltCount = (endTilt - startTilt) / stepTilt + 1
    processing_directory = os.path.join(output_directory, basename + '_Processing/')
    f = open(tiltFile, 'w')
    f.write(str(tiltCount) + '\n')
    for tilt in range(startTilt, endTilt+1, stepTilt):
        output_name = os.path.join(output_directory, getSubtiltFilename(basename, subtilt, tilt))
        f.write(output_name + '\n')
        f.write('0\n')
    f.close()

def createRawTilt(startTilt, endTilt, stepTilt, rawTiltFile):
    ''' Create user.rawtlt '''
    f = open(rawTiltFile, 'w')
    for tilt in range(startTilt, endTilt+1, stepTilt):
        f.write(str(tilt) + '\n')
    f.close()

def splitTilt(basename, seqnum_start, period, tilt, input_directory, output_directory):
    ''' find all movies match a tilt angle and copy each to the respective subtilt folder renamed '''
    expected_name = '%s_*_%d.0.*.mrc' % (basename, tilt)
    patterns = [ expected_name ]
    if tilt == 0:
        patterns.append('%s_*_-%d.0.*.mrc' % (basename, tilt))
    # We can find all the files of a tilt angle, then put then in folders without index.
    regex_pattern = re.compile(basename + '_(\d+)_.*.mrc')
    for filename in os.listdir(input_directory):
        for pattern in patterns:
            if (fnmatch.fnmatch(filename, pattern)):
                ''' now need to get the index value '''
                m = regex_pattern.search(filename)
                if (m): 
                    subtilt = (int(m.group(1)) - seqnum_start + 1) % period
                    if (subtilt == 0):
                        subtilt = period
                    print(str(tilt) + ' found ' + filename + ' with index ' + m.group(1) + ' copying to ' + str(subtilt))
                    output_name = os.path.join(output_directory, 'subTilt_' + str(subtilt), getSubtiltFilename(basename, subtilt, tilt) )
                    src = os.path.join(input_directory, filename)
                    shutil.copyfile(src, output_name)

def main():
    # 1. Provide commandline options
    parser = argparse.ArgumentParser(description=description_text)

    ## Parameters
    parser.add_argument('--input', help='path to the data collection directory', required=True, default=None)
    parser.add_argument('--output', help='path to a location to write results', required=True, default=None)
    parser.add_argument('--basename', help='define a common basename for the images', required=True, default=None)
    parser.add_argument('--period', help='define number of subtilts', type=int, required=True, default=None)
    parser.add_argument('--seqnum_start', help='sequential starting number (000)', type=int, required=True, default=000)
    parser.add_argument('--starting_angle', help='define the minimal bounds of the tilt range, ex. -60', type=int, required=True, default=None)
    parser.add_argument('--ending_angle', help='define the maximal bounds of the tilt range, ex. 60', type=int, required=True, default=None)
    parser.add_argument('--tilt_increment', help='define the increment of the tilt, ex 3', type=int, required=True, default=None)
    args = parser.parse_args()

    # Need to separate X subtilts at each tilt angle into separate folder. This represents an offset.
    window_start = args.seqnum_start

    ## Make the subtilt directories
    for subtilt in range(1, args.period + 1):
        outputTiltDir = os.path.join(args.output, 'subTilt_' + str(subtilt))
        if (not os.path.isdir(outputTiltDir)):
            os.makedirs(outputTiltDir)
    
    ## Split the tilt files for each subtilt into a separate directory, and stack.
    for tilt in range(args.starting_angle, args.ending_angle+1, args.tilt_increment):
        splitTilt(args.basename, args.seqnum_start, args.period, tilt, args.input, args.output)

    ## Build a stack in each subtilt directory.
    for subtilt in range(1, args.period+1):   
        outputTiltDir = os.path.join(args.output, 'subTilt_' + str(subtilt))   
        rawTiltTxt = os.path.join(outputTiltDir, 'rawtlt.txt')
        createRawTilt(args.starting_angle, args.ending_angle, args.tilt_increment, rawTiltTxt)

        tiltListTxt = os.path.join(outputTiltDir, 'tiltList.txt')
        createTiltList(outputTiltDir, args.basename, subtilt, args.starting_angle, args.ending_angle, args.tilt_increment, tiltListTxt)

        # After copying the subtilts, should be renamed $basename_subtilt_tilt.0.mrc
        finalStackFile = os.path.join(outputTiltDir, args.basename + '_subtilt_' + str(subtilt) + '_' + str(tilt) + '_AliSB.st')
        print("Building stack for subtilt 1 ")

        # Generate usertilt.txt
        subprocess.run(['newstack', '-tilt', rawTiltTxt, '-fileinlist', tiltListTxt, '-ou', finalStackFile])


if __name__ == "__main__":
    main()
