## Bash Scripts for Montage Tomography Processing.

If the MPACT dataset is collected in SerialEM 4.0, we recommend the user apply [Python Scripts](https://github.com/wright-cemrc-projects/cryoet-montage/tree/main/Python) to perform the automated preprocessing steps including motion corrections, montaged and individual tile tilt series generation. If the MPACT dataset is collected in SerialEM 4.1, we recommend using the scripts here and steps listed below that serve as an alternative way for most updated versions. 

### Getting the scripts

Clone the GitHub repository with `git clone https://github.com/wright-cemrc-projects/cryoet-montage.git`

### Requirements

You will need to have MATLAB(MATLAB_R2020b), IMOD 4.11.6 or higher, motioncor2 if motion correction of raw movie stacks are desired installed. 

## *Using coordinate_mpact_SerialEM.m or coordinate_mpact_SerialEM4_1.m*
Download [`coordinate_mpact_SerialEM.m`](coordinate_mpact_SerialEM.m) if the montage tilt series are collected via SerialEM 3.8 and above stable release or [`coordinate_mpact_SerialEM4_1.m`](coordinate_mpact_SerialEM4_1.m) if the montage tilt series are collected via SerialEM 4.1. Place the function files in the same directory where MATLAB desktop or online is launched.

In the MATLAB command window, you could type `help coordinate_mpact_SerialEM.m` or `help coordinate_mpact_SerialEM4_1.m` to know the input parameters.
An example command to generate the coordinate files for a 3x3 mpact tilt series acquired on a standard Gatan K3 camera with a root name of *3x3_mpact_ts_1*:

```
coordinate_mpact_SerialEM4_1(3,3,576,408,5760,4092,-60,60,3,'3x3_mpact_ts_1')
```
```
coordinate_mpact_SerialEM(3,3,576,408,5760,4092,-60,60,3,'3x3_mpact_ts_1')
```

An example output:
```
3x3_mpact_ts_1_-60.pl
3x3_mpact_ts_1_-57.pl
...
3x3_mpact_ts_1_57.pl
3x3_mpact_ts_1_60.pl
```
Currently, the [`coordinate_mpact_SerialEM.m`](coordinate_mpact_SerialEM.m) and [`coordinate_mpact_SerialEM4_1.m`](coordinate_mpact_SerialEM4_1.m) should directly generate the correct piece coordinate files (.pl). For older versions of the scripts, you may need to rename the coordinate files from .txt to .pl by running the following commands in the terminal:
```
for f in 3x3_mpact_ts_1*.txt;do mv ${f} ${f/\.txt/\.pl};done
```
You will get the final coordinate files that will be used for the montage stitching process:
```
3x3_mpact_ts_1_-60.pl
3x3_mpact_ts_1_-57.pl
...
3x3_mpact_ts_1_57.pl
3x3_mpact_ts_1_60.pl
```
Place all corresponding .pl files in individual directories containing frames for one montage tilt series (see example below).

## *Using blendstitching_tiltcompensated.sh*
Download [`blendstitching_tiltcompensated.sh`](blendstitching_tiltcompensated.sh) and place in individual directories containing frames for one montage tilt series with file frames like:

`3x3_mpact_ts_1_000_-0.0.tif`

This naming includes an serial sequential number starting at 000 and increasing with each collected image. Immediately before the .tif extension is a -0.0 which is the tilt angle of this particular movie. Tiff format files are generally raw movie stacks that require motion correction. The motion correction option is included in the [`blendstitching_tiltcompensated.sh`](blendstitching_tiltcompensated.sh)

Run the bash script interactively. An example run where the raw movie stack (3x3 MPACT tilt series from -60 degree to 60 degrees with a 3 degree increment, via a Gatan K3 camera with a configuration of 180 degree rotation and flip around Y) will need to be aligned/motion corrected via motioncorr2:
```
bash blendstitching_tiltcompensated.sh
```
Inputs will be prompted interactively. The prompts and inputs (starts with #):
```
put the bash script blendstitching_updated.sh and all piece coordinate files in the same folder where all frames in motion_corrected mrc format or unaligned tiff are
which basename
# 3x3_mpact_ts_1
starting angle negative e.g. -60
# -60
ending angle positive e.g. 60
# 60
tilt increments e.g. 3
# 3
location to transfer data full path or NA
# NA
do you need to generate basename folder (y/n)
# y
Is motion correction needed and input files are tiff(y/n)
# y
which pixel size in Angstroms
# 4.603
which gain
# CountCDSRef_3x3_mpact_ts_1_000_-0.0.mrc
which rotation e.g.2 for 180 rotation
# 2
which flip e.g. 2 for flip around vertical axis
# 2
```
When the run finishes, you will see motion corrected frame per tilt *3x3_mpact_ts_1_000_-0.0.tif.mc.mrc*, three MRC format stack files including the unbinned stitched montage tilt series *3x3_mpact_ts_1_AliSB.st*, bin2 stack *3x3_mpact_ts_1_AliSB_bin2.st* and bin4 stack *3x3_mpact_ts_1_AliSB_bin2.st*. All three stitched tilt series stacks are ready for tomogram generations. There is also a folder *3x3_mpact_ts_1_Processing* where invidividual tile stitching per tilt are sorted and ready for any manual adjustment if needed by following [Midas](https://github.com/wright-cemrc-projects/cryoet-montage/tree/main/Midas).

## *Using split_tiltseries.sh*
Download [`split_tiltseries.sh`](split_tiltseries.sh) and place in individual directories containing frames for one montage tilt series as above. Run the bash script interactively as well.
```
bash split_tiltseries.sh
```
Inputs will be prompted interactively. The prompts and inputs (starts with #):
```
subtilt first (e.g.1)
# 1
subtilt last (3x3 be 9)
# 9
sequential starting number (000)
# 000
sequential ending number (000)
# 368
basename
# 3x3_mpact_ts_1
how many subtilts (3x3 be 9)
# 9
starting angle negative e.g. -60
# -60
ending angle positive e.g. 60
# 60
tilt increments (e.g. 2 or 3)
# 3
location to transfer data full path or NA
# NA
Is motion correction needed and input files are tiff(y/n)
# y
which pixel size in Angstroms
# 4.603
which gain
# CountCDSRef_3x3_mpact_ts_1_000_-0.0.mrc
which rotation e.g.2 for 180 rotation
# 2
which flip e.g. 2 for flip around vertical axis
# 2
```
When the run finishes, you will see a series of folders cooresponding to each tile tilt series and sorted tile series stacks e.g. *3x3_mpact_ts_1_subtilt_1_AliSB.st* ready for tomogram reconstructions. 
