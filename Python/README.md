## Python Scripts for Montage Tomography Processing.

### Getting the scripts

Navigate to the Releases to get the latest package containing Python scripts. 

### Requirements
Your environment will need to have Python 3 installed and in the system PATH.

You also need an installation of IMOD 4.11.6 or higher to provide the tools for assembling stacks and blending images.

## *Using BlendStitch.py*
After a collection of a montage with SerialEM, you will have a single directory containing frames with filenames like:

`W1618_G3_Pt21_3x3_tilt_2_000_-0.0.mrc`

This naming includes an serial index starting at 000 and increasing with each collected image. Immediately before the .mrc extension is a -0.0 which is the tilt angle of this particular movie.

From the filenames you need to determine the basename of all the images such as `W1618_G3_Pt21_3x3_tilt_2`, a prefix that is shared by all the files.

Running the `BlendStitch.py --help` provides a listing of the possible command-line arguments to provide the Python wrapper for blending montage images:

```
./BlendStitch.py --help
usage: BlendStitch.py [-h] --input INPUT --output OUTPUT --basename BASENAME
                      --starting_angle STARTING_ANGLE --ending_angle
                      ENDING_ANGLE --tilt_increment TILT_INCREMENT
                      [--camera_x CAMERA_X] [--camera_y CAMERA_Y]
                      [--overlap_x OVERLAP_X] [--overlap_y OVERLAP_Y]
                      [--tile_x TILE_X] [--tile_y TILE_Y]

BlendStitch: blend and stitch tiled images from tilt-series into a larger
image and stack.

optional arguments:
  -h, --help            show this help message and exit
  --input INPUT         path to the data collection directory
  --output OUTPUT       path to a location to write results
  --basename BASENAME   define a common basename for the images
  --starting_angle STARTING_ANGLE
                        define the minimal bounds of the tilt range, ex. -60
  --ending_angle ENDING_ANGLE
                        define the maximal bounds of the tilt range, ex. 60
  --tilt_increment TILT_INCREMENT
                        define the increment of the tilt, ex 3
  --camera_x CAMERA_X   define camera width in pixel dimensions (default 5760)
  --camera_y CAMERA_Y   define camera height in pixel dimensions (default
                        4092)
  --overlap_x OVERLAP_X
                        define a percent overlap where 15 percent would be
                        0.15 (default 0.15)
  --overlap_y OVERLAP_Y
                        define a percent overlap where 10 percent would be
                        0.10 (default 0.10)
  --tile_x TILE_X       define the number of tiles in the x dimension (default
                        3)
  --tile_y TILE_Y       define the number of tiles in the y dimension (default
                        3)
```

### Example run

Assuming you have an input directory containing a 3x3 montage, and you want to produce the blended image at each tilt angle and assemble this into a stack, you could build a command like below:

```
./BlendStitch.py --input W1618_G3_Pt21_3x3_tilt_2_init/ \ 
--output W1618_G3_Pt21_3x3_tilt_2_init_out --basename W1618_G3_Pt21_3x3_tilt_2 \
--starting_angle -51 --ending_angle 51 \
--tilt_increment 3 \
--overlap_x 0.2 --overlap_y 0.15 \
--tile_x 3 --tile_y 3
```

This example would process a tilt series from -51 to 51 degrees tilt angles. This is assuming a K3 camera with a 5760x4092 pixel dimension and asking for overlaps of 20% on the X-axis between neighboring images and 15% on the y-axis for neighboring images.

The end output is a series of *.st files with no-binning, bin of 2, and bin of 4 which can be further processed to generate a tomogram.

## *Using SplitTomogram.py*

Alternatively, you may want to pull out individual subtilt directories and build separate tomograms for each subtilt. Here, a subtilt is the same image shift position collected each tilt angle that can be combined as an independent stack of tilt images.

```
./SplitTomogram.py --help
usage: SplitTomogram.py [-h] --input INPUT --output OUTPUT --basename BASENAME
                        --period PERIOD --seqnum_start SEQNUM_START
                        --starting_angle STARTING_ANGLE --ending_angle
                        ENDING_ANGLE --tilt_increment TILT_INCREMENT

SplitTomogram: split the tiled images from a tilt-series into a individual
images and stack.

optional arguments:
  -h, --help            show this help message and exit
  --input INPUT         path to the data collection directory
  --output OUTPUT       path to a location to write results
  --basename BASENAME   define a common basename for the images
  --period PERIOD       define number of subtilts
  --seqnum_start SEQNUM_START
                        sequential starting number (000)
  --starting_angle STARTING_ANGLE
                        define the minimal bounds of the tilt range, ex. -60
  --ending_angle ENDING_ANGLE
                        define the maximal bounds of the tilt range, ex. 60
  --tilt_increment TILT_INCREMENT
                        define the increment of the tilt, ex 3
```

### Example

```
./SplitTomogram.py --input W1618_G3_Pt21_3x3_tilt_2_init \
--output W1618_G3_Pt21_3x3_tilt_2_init_out/ \
--basename W1618_G3_Pt21_3x3_tilt_2 --period 9 \
--seqnum_start 000 --starting_angle -51 --ending_angle 51 --tilt_increment 3
```

In this example, a 3x3 montage would have a total of 9 shots per tilt angle. This defines a parameter --period 9, which is the pattern of how SerialEM will return back to the same subtilt in the next tilt angle.

The above command will result in the creation of an output folder with subfolders for each subtilt and an assembled stack within each subtilt directory.
