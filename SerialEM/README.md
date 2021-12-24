# SerialEM set up for Montage Cryo-Electron Tomography

This workflow uses SerialEM to implement dose distribution and montage cryo-electron tomography for acquisition of large field of view without sacrificing high resolution information.

## *Features*

- Flexibility. The current tools support rectangle or square montage tiling pattern. Users can specify the tile piece numbers based on their applications

- Outputs. The acquired montage tilt series can be stitched together to be used as as one whole unit or sorted into individual tiles to be used as regular tomograms. One 3x3 montage tilt series could yield 10 usable tilt series (9 individual tiles and 1 fully stitched tilt series) 

- Acquisition. This workflow uses the built-in SerialEM function, *Multiple Record* with image shift, to achieve accurate image-shift based montage pattern collection. Thus, all settings and functions related to *Multiple Record* are applicable here to design user-specified patterns, correct coma introduced by beam tilt, etc.

## *Prerequisites*

Stable beam-image shift lens system. In general, the set up should be applicable to all autoloader-equipped microscopes. 

## ***Microscope imaging set up***

1. The image shift and pixel size at the recording magnification should be well calibrated and up to date in SerialEM, follow the instructions [Calibrate Image Shift and Pixel Size](https://bio3d.colorado.edu/SerialEM/hlp/html/setting_up_serialem.htm#setup_pixelsize).

2. Generate regular patterns with image shift.

   - Go to an area with contents e.g. samples or region of interest, reset image shift using 'Reset Image Shift' collect an image-shift 2x2 montage at the recording magnification and camera binning that will later be used for tilt series acquisitions. 

   - Specify the piece numbers of x and y and desired overlaps in pixels in [Montage Setup Dialog](https://bio3d.colorado.edu/SerialEM/hlp/html/hidd_montagesetup.htm).
     - e.g. We benchmarked 15% to 20% in fringe-affected axis (x-axis) and 10% in fringeless axis (y-axis). For a full frame K3 camera (bin 1, 5760 x 4092), it is 864 (15% of 5760) or 1152 (20% of 4092) in x and 409 (10% of 4092) in y as inputs of 'Overlap in X and Y' in the 'Montage Setup Dialogue'. 
   - Save the ".mdoc" file.
   - Retrieve the image shift information in the .mdoc file to create a m * n tiling pattern with desired overlaps in x and y.
       - Image shift in the x direction = ImageShift of section 2 - ImageShift of section 0   
         
e.g. you may see in the .mdoc file of the 2x2 montage with a section like below

'[ZValue = 0]
PieceCoordinates = 0 0 0
MinMaxMean = 0 1014 79.9226
TiltAngle = 0.00427435
StagePosition = 265.414 118.065
StageZ = 49.8675
Magnification = 19500
Intensity = 0.120144
ExposureDose = 1.66243
DoseRate = 2.86627
PixelSpacing = 4.603
SpotSize = 8
Defocus = -1.09678
ImageShift = -2.51756 0.229891
RotationAngle = 175.28
ExposureTime = 1.00134
Binning = 1
CameraIndex = 2
DividedBy2 = 0
OperatingMode = 1
UsingCDS = 1
MagIndex = 24
LowDoseConSet = 9
CountsPerElectron = 32
TargetDefocus = -5
DateTime = 20-Oct-21  16:30:54
FilterSlitAndLoss = 20 0
AlignedPieceCoords = -7 -10 0
XedgeDxy = -70.1522 50.0197
YedgeDxy = 14.8435 -32.5203'

Section 0 labeled as ZValue = 0 has the ImageShift entry of -2.51756 0.229891 while section 2 labeled as Value = 2 has the ImageShift entry of -0.498705 1.03549. To achieve the overlap, the image shift in the x direction should be 2.0189 0.8056
         
       - Image shift in the y direction = ImageShift of section 1 - ImageShift of section 0
         
         - e.g. Section 1 labeled as 'ZValue = 1' has the 'ImageShift' entry of "-1.91004 -1.26029". The image shift in the y direction should be '0.6075 -1.4902'
   - Plug in the required image shifts in 'MultishotParams' in the SerialEM setting files and save . 
       
       - Under the section of 'MultishotParams', 'X or Y component of image shift vector in X or Y direction' control the overlap in the x and y direction.

e.g. when you open the serialEM setting file, you may see a line like this 

       'MultiShotParams 0.200000 0.500000 2 1 0 0 0 2.000000 1 1 2 0 1.500000 2.001000 0.768026 0.603000 -1.473200 3 3 24 19 0 0 3 0.250000 -0.020223 -999.000000 -999.000000'
              
The numbers correspond to 28 parameters associated with *MultishotParams. You will update item 14 to 17 by manually updating the image shift entries. The updated result will be like this

       'MultiShotParams 0.200000 0.500000 2 1 0 0 0 2.000000 1 1 2 0 1.500000 2.0189 0.8056 0.6075 -1.4902 3 3 24 19 0 0 3 0.250000 -0.020223 -999.000000 -999.000000'

You will also update item 18 and 19 to specify the size of the regular pattern (square or rectangle) you would like to have, e.g. 3 x4, then it should like this in the end

       'MultiShotParams 0.200000 0.500000 2 1 0 0 0 2.000000 1 1 2 0 1.500000 2.0189 0.8056 0.6075 -1.4902 3 4 24 19 0 0 3 0.250000 -0.020223 -999.000000 -999.000000'

Note: the serialEM setting file cannot be updated if the file is being open in SerialEM. If you would like to update the loaded file, save the current file, make a copy and update the copy and then reload the updated copy version.

3. Set up a series of ROI by saving the 'View' or 'Preview' shots as anchor maps and turn on 'Acquire' in the Navigator.
4. Edit the parameters in the 'cryoMontage.txt' macro.

Note: We find the 'View' shot at a magnification of 2000x to 6500x (EFTEM), pixel size between 33.9 to 13.6 Å on a Titan Krios has been robust enough to achieve good realignment of ROI during the automated tilt series collection. 




