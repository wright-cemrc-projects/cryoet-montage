## Bash Scripts for Montage Tomography Processing.

If the MPACT dataset is collected in SerialEM 4.0, we recommend the user apply [Python Scripts](https://github.com/wright-cemrc-projects/cryoet-montage/tree/main/Python) to perform the automated preprocessing steps including motion corrections, montaged and individual tile tilt series generation. If the MPACT dataset is collected in SerialEM 4.1, we recommend using the scripts here and steps listed below that serve as an alternative way for most updated versions. 

### Getting the scripts

Clone the GitHub repository with `git clone https://github.com/wright-cemrc-projects/cryoet-montage.git`

### Requirements

You will need to install MATLAB(MATLAB_R2020b or higher), IMOD 4.11.6 or higher, and MotionCor2 if motion correction of raw movie stacks is desired. To perform the motion correction function in the bash script [`blendstitching_tiltcompensated_batch.sh`](blendstitching_tiltcompensated_batch.sh) correctly, you will need to identify the camera orientation set up in SerialEM. This can be checked via the "SerialEMproperties.txt" file located in the "ProjectData" folder on the K3 or microscope PC where SerialEM is installed. The camera orientation parameters are generally system-dependent. For example, on our K3 camera, we need to apply 180-degree rotation and flip Y in the motion correction process. On our Falcon 4 camera, the raw EER frames from the camera are flipped vertically and rotated 180 degrees (No flip or rotation applied in SerialEM in this case). Motioncor2 applies a vertical flip during the frame alignment, so a final 180-degree rotation is needed post-Motioncor2. Noted, MotionCor2 installation might be done via SBGrid. The script will prompt you to confirm the option during the interactive run. In this case, the environmental variable for MotionCor2 is motioncor2. 

Some modifications might be needed for each system. Please feel free to reach out via email (jyang525@wisc.edu). 

## *Using coordinate_mpact_SerialEM.m or coordinate_mpact_SerialEM4_1.m*
Download [`coordinate_mpact_SerialEM.m`](coordinate_mpact_SerialEM.m) if the montage tilt series is collected via SerialEM 3.8 and above stable release or [`coordinate_mpact_SerialEM4_1.m`](coordinate_mpact_SerialEM4_1.m) if the montage tilt series is collected via SerialEM 4.1. Place the function files in the same directory where MATLAB desktop or online is launched.

In the MATLAB command window, you could type `help coordinate_mpact_SerialEM.m` or `help coordinate_mpact_SerialEM4_1.m` to know the input parameters.
An example command to generate the coordinate files for a 3x3 MPACT tilt series acquired on a standard Gatan K3 camera with a root/base name of *3x3_mpact_ts_1*:

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
Place all corresponding .pl files in individual directories containing raw frame stacks for one montage tilt series. For example, all .pl files with a root/base name of *3x3_mpact_ts_1* need to be placed in the folder "3x3_mpact_ts_1" (see example below).

## *Using blendstitching_tiltcompensated_batch.sh*
Download [`blendstitching_tiltcompensated_batch.sh`](blendstitching_tiltcompensated_batch.sh) and place it in the directory that is above individual directories containing frames for one montage tilt series.

When listing the directory where the [`blendstitching_tiltcompensated_batch.sh`](blendstitching_tiltcompensated_batch.sh)  is located:
run the command in the terminal below:
```
ls -1
```
You will see an example layout as below:
```
blendstitching_tiltcompensated_batch.sh
3x3_mpact_ts_1
3x3_mpact_ts_2
...
```
When listing one montage tilt series folder, run
```
ls 3x3_mpact_ts_1
```
You will see each raw frame stack listed in the terminal, for example:
```
3x3_mpact_ts_1_000_-0.0.tif
3x3_mpact_ts_1_001_-0.0.tif
3x3_mpact_ts_1_002_-0.0.tif
3x3_mpact_ts_1_003_-0.0.tif
3x3_mpact_ts_1_004_-0.0.tif
3x3_mpact_ts_1_005_-0.0.tif
3x3_mpact_ts_1_006_-0.0.tif
3x3_mpact_ts_1_007_-0.0.tif
3x3_mpact_ts_1_008_-0.0.tif
3x3_mpact_ts_1_009_3.0.tif
3x3_mpact_ts_1_010_3.0.tif
...
```
Following the base name *3x3_mpact_ts_1*, the frame stack naming includes a serial sequential number starting at 000, which increases sequentially, and the tilt angle of this particular movie, e.g. -0.0, 3.0., followed by the .tif extension. Tiff format files are generally raw movie stacks that require motion correction. If motion correction is needed for MPACT, you will need to check the camera orientation set up as mentioned above, before running [`blendstitching_tiltcompensated_batch.sh`](blendstitching_tiltcompensated_batch.sh). 

Next, we will generate a simple text file that lists all montage tilt series folders per row. This is the last step of preparation.

Run the command below:
```
ls -1 >> folderlist.txt
```
A text file named "folderlist.txt" will be generated. 
Check the file content

Run the command in the terminal below:
```
cat -n folderlist.txt
```
You will see the "folderlist.txt" content listed in the terminal, for example:
```
1	blendstitching_tiltcompensated_batch.sh
2	folderlist.txt
3	3x3_mpact_ts_1
4	3x3_mpact_ts_2
5	3x3_mpact_ts_3
6	3x3_mpact_ts_4
```
Command *cat -n* numbers all output files. The first column is the number index. In this example, there are four 3x3 MPACT tilt series folders *3x3_mpact_ts_1*, *3x3_mpact_ts_2*, *3x3_mpact_ts_3*, and *3x3_mpact_ts_4*, the bash script, and the "folderlist.txt" in the current directory. So, there are in total 6 outputs numbered 1 to 6. The second column is the output file, one file per row. 

Run the bash script interactively. An example run is listed below. Here, we will only process *3x3_mpact_ts_1* and *3x3_mpact_ts_2*. Based on the content in "folderlist.txt", *3x3_mpact_ts_1* has a number index of 3, and *3x3_mpact_ts_2* has a number index of 4. The raw movie stacks are collected on a Gatan K3 camera with a configuration of 180-degree rotation and flipped around Y. Each MPACT tilt series is from -60 degrees to 60 degrees with a 3-degree increment.

Let's start the run:
```
bash blendstitching_tiltcompensated_batch.sh
```
Inputs will be prompted interactively. The prompts and inputs (starts with $):
```
starting tilt series index
$ 3
ending tilt series index
$ 4
starting angle negative e.g. -60
$ -60
ending angle positive e.g. 60
$ 60
tilt increments e.g. 3
$ 3
Is motion correction needed (type y for MotionCor2/type SBgrid for motioncor2)
$ y
camera, type K3 for tif format collected using K3 or type F4 for eer format collected using Falcon4
$ k3
tiff only, rotation of the gain applied, 0 no rotation, 1 to 3 refer to 90, 180, 270 or NA
$ 2
tiff only, flip gain applied, 0 no flipping, 1 flip upside down, 2 flip left and right or NA
$ 2
unbinned pixel size
$ 4.603
```
When the run finishes, you will see motion corrected frame per tilt *3x3_mpact_ts_1_000_-0.0.tif.mc.mrc*, three MRC format stack files including the unbinned stitched montage tilt series *3x3_mpact_ts_1_AliSB.st*, bin2 stack *3x3_mpact_ts_1_AliSB_bin2.st* and bin4 stack *3x3_mpact_ts_1_AliSB_bin2.st*. All three stitched tilt series stacks are ready for tomogram generations. There is also a folder *3x3_mpact_ts_1_Processing* where individual tile stitching per tilt is sorted and ready for any manual adjustment if needed by following [Midas](https://github.com/wright-cemrc-projects/cryoet-montage/tree/main/Midas).

## *Using split_tiltseries.sh*
Similarly, download [`split_tiltseries_batch.sh`](split_tiltseries_batch.sh) and place it in the directory one level above individual directories containing frames for one montage tilt series. Noted, if all MPACT tilt series share the same dimension of m x n (could be 3x4 or 4x3), this batch script run can process them in one interactive run. If they have different dimensions (e.g. some are 12 (3x4 or 4x3) and others are 4 (2x2) or 3x3 (3x3)), modify the "folderlist.txt" via common text editors such as Nano, Vim, Gedit, or others, to group them by dimension and run the processing in multiple interactive runs. 

When listing the directory where the [`split_tiltseries_batch.sh`](split_tiltseries_batch.sh) is located:
run the command in the terminal below:
```
ls -1
```
You will see an example layout as below:
```
split_tiltseries_batch.sh
3x3_mpact_ts_1
3x3_mpact_ts_2
...
```
Similarly, generate the "folderlist.txt" and specify which tilt series to process, following the steps above by running
```
ls -1 >> folderlist.txt
```
and
```
cat -n folderlist.txt
```
You will see an example below:
```
1	split_tiltseries_batch.sh
2	folderlist.txt
3	3x3_mpact_ts_1
4	3x3_mpact_ts_2
5	3x3_mpact_ts_3
6	3x3_mpact_ts_4
7	4x3_mpact_ts_1
8	4x3_mpact_ts_2
9	3x4_mpact_ts_3
10	2x2_mpact_ts_1
```
Here, the "folderlist.txt" has been modified so the same dimension MPACT tilt series are grouped. The index numbers 3 to 6 refer to the 3x3 dimension group and index numbers 7 to 9 refer to the 4x3 or 3x4 dimension group. The dimension determines the starting and ending serial sequential numbers required for the sorting, as explained below.

Before you start the interactive run, one more thing to check is the starting and ending serial sequential numbers of frame stacks. For example, to check the numbers in the dimension of 3x3 MPACT group via using the first "3x3_mpact_ts_1" folder, run the following command

```
ls 3x3_mpact_ts_1
```
You will see each raw frame stack listed in the terminal, for example:
```
3x3_mpact_ts_1_000_-0.0.tif
3x3_mpact_ts_1_001_-0.0.tif
3x3_mpact_ts_1_002_-0.0.tif
3x3_mpact_ts_1_003_-0.0.tif
3x3_mpact_ts_1_004_-0.0.tif
3x3_mpact_ts_1_005_-0.0.tif
3x3_mpact_ts_1_006_-0.0.tif
3x3_mpact_ts_1_007_-0.0.tif
3x3_mpact_ts_1_008_-0.0.tif
3x3_mpact_ts_1_009_3.0.tif
3x3_mpact_ts_1_010_3.0.tif
...
3x3_mpact_ts_1_368_60.0.tif
```
Here, the starting sequential number is 000 and the ending sequential number is 368. You can calculate the ending sequential number based on the dimension and total tilts. For example, the dimension of a 3x3 MPACT is 9 and the total tilts for an MPACT tilt series of -60 degrees to 60 degrees with a 3-degree increment are 41. With the starting sequential number being 000 (instead of 001), the ending sequential number should be 9 x 41 - 1 = 368. Sometimes, the MPACT might be stopped prematurely by the user or the user might define a different starting sequential number in the "Frame File Options Dialog" in SerialEM. It is recommended to check/confirm the starting and ending sequential numbers as above, and to remove duplicate frames if you know there might be some. For example, 3x3 MPACT acquisition can be paused at the stage tilt of 6 degrees and then resumed by the user who may choose to recollect at the stage tilt of 6 degrees with modified Record parameters. Thus, there might be 24 frames associated with the 6 degrees. The unwanted duplicates will then need to be removed and the starting and ending sequential numbers might not be 000 or 368. 

Now we can start the run interactively to sort and split "3x3_mpact_ts_1".
```
bash split_tiltseries_batch.sh
```
Inputs will be prompted interactively. The prompts and inputs (starts with $):
```
starting tilt series index
$ 3
ending tilt series index
$ 3
subtilt first (e.g.1)
$ 1
subtilt last (3x3 be 9)
$ 9
sequential starting number (000)
$ 000
sequential ending number (000)
$ 368
how many subtilts (3x3 be 9)
$ 9
starting angle negative e.g. -60
$ -60
ending angle positive e.g. 60
$ 60
tilt increments (e.g. 2 or 3)
$ 3
Is motion correction needed (type y for MotionCor2/type SBgrid for motioncor2/type n for no correction)
$ SBgrid
camera, type K3 for tif format collected using K3 or type F4 for eer format collected using Falcon4
$ K3
tiff only, rotation of the gain applied, 0 no rotation, 1 to 3 refer to 90, 180, 270 or NA for Falcon4
$ 2
tiff only, flip gain applied, 0 no flipping, 1 flip upside down, 2 flip left and right or NA for Falcon4
$ 2
unbinned pixel size
$ 4.603
location to transfer data full path or NA
$ NA
```
When the run finishes, you will see a series of folders corresponding to each tile tilt series and sorted tile series stacks e.g. *3x3_mpact_ts_1_subtilt_1_AliSB.st* ready for tomogram reconstructions. 
