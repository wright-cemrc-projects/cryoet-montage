#!/bin.bash

##scripts for CryoMontage tilt series MPACT by JaeYang from Wright Lab Oct. 2021, last edited 6_7_2023

echo CryoMontage montage tilt series stitching and new stack generation

###### function #################

# copy a second set of aligned frames output from function alignframes to a new folder xxx_Processing
# organize the frames into individual tilts for subsequent inspections and Midas launching
# SerialEM frames have some fixed naming, e.g. xxx_-6.0.mrc (.0. added between tilt angle -6 and file appendix .mrc)
 
stitching () {
 while [ $i -le $end ];
 do
  # make individual folders for each tilt to store original frames and stitched tiles per tilt
   mkdir ${Basename}_Processing/Tilt_${i}
  # Depending on the collection schemes, 0 tilt could be written as -0 or 0 in the file name
  # ignore the erro if 0 or -0 doesn't exist
   if [ $i == "0" ]
    then 
      cp ${Basename}_*_0.0.*.mrc ${Basename}_Processing/Tilt_0/
      cp ${Basename}_*_-0.0.*.mrc ${Basename}_Processing/Tilt_0/
      cp ${Basename}_0.pl ${Basename}_Processing/Tilt_0/      
  else
      cp ${Basename}_*_${i}.0.*.mrc ${Basename}_Processing/Tilt_${i}/
  # piece coordinate files needed for blendmont and also ready for subsequent inspection and Midas launching
      cp ${Basename}_${i}.pl ${Basename}_Processing/Tilt_${i}/
    fi;

  # generate a new stack per tilt for stitching (xxx_-6.mrc)  and stitch the stack in individual tilt folders called xxx_-6_blend.st using blendmont command
  # copy a stitched stack per tilt to the upper directory for whole montage tilt series generation

  newstack ${Basename}_Processing/Tilt_${i}/*.mrc ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.st

  # use blendmont command includes both edge function calculation and cross-correlation 
  blendmont -imin ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.st -imout ${Basename}_Processing/Tilt_${i}/${Basename}_${i}_blend.st -plin ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.pl -roo ${Basename}_Processing/Tilt_${i}/${Basename}_${i} -sloppy
  cp ${Basename}_Processing/Tilt_${i}/${Basename}_${i}_blend.st ${Basename}_Processing/

  # remane the piece coordinate files to match with the pre-blend new stack to avoid errors in Midas
  # mv ${Basename}_Processing/Tilt_${i}/${Basename}.pl ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.pl
  i=$(($i+$c))
 done
  # rm ${Basename}.pl
}


## generate a txt file that follows the required format and contains the file names to be used in newstack command

userlist() {
      while [ $i -le $end ]
         do
           echo ${Basename}_${i}_blend.st >> tiltlist.txt 
           echo 0 >> tiltlist.txt
           i=$(($i+$c))
      done
}

## generate a txt file with tilt angles information to be inserted into the tilt series 

usertilt() {
      while [ $i -le $end ]
        do
         echo $i >> tilt.rawtlt
         i=$(($i+$c))
      done
}

## align frames with motioncorr2 if frames are not aligned yet

motioncorr() {
    echo "Is motion correction needed and input files are tiff(y/n)"
    read answer
    if [ $answer == "y" ]
     then
       echo which pixel size in Angstroms
       read pixelsize
       echo which gain
       read gain
       echo "which rotation e.g.2 for 180 rotation"
       read rotgain
       echo "which flip e.g. 2 for flip around vertical axis"
       read flipgain
       motioncor2 -InTiff ${f} -OutMrc ${f}.mc.mrc -Patch 5 5 -PixelSize $pixelsize -Gpu 0 1 2 3 -Gain $gain -RotGain $rotgain -FlipGain $flipgain
     else
       echo no motion correction
     fi
}

## main script start

echo "============main  process starting=============="
echo "put the bash script blendstitching_updated.sh and all piece coordinate files in the same folder where all frames in motion_corrected mrc format or unaligned tiff are"

 echo "which basename" 
 read Basename
 echo "starting angle negative e.g. -60"
 read i
 echo "ending angle positive e.g. 60"
 read end
 echo "tilt increments e.g. 3"
 read c
 echo "location to tranfer data full path or NA"
 read fullpath
 echo "do you need to generate basename folder (y/n)"
 read Processing

# motion correction if needed 
 motioncorr

 tilts=$(($end/$c))
 tilts=$(($tilts*2+1))

 origin=$i

# generate folder to hole all of the files
  mkdir ${Basename}_Processing

# perfom stitching function
 stitching;

 location=$(pwd)

# go to the processing folder and generate the stitched tilt series 

 cd $location/${Basename}_Processing/

 namechange;

 i=$origin

# generate a premature tiltlist file to hold the file names and used in function userlist to create final tiltlist.txt
 
 echo $tilts >> tiltlist.txt
 userlist;

# reset i for function usertilt
 i=$origin

# generate a list of tilt angles to insert into the stitched tilt series
 usertilt;

# generate the stitched nxn unbinned tilt series
 newstack -tilt tilt.rawtlt -fileinlist tiltlist.txt -ou ${Basename}_AliSB.st

# generate a binned 2x and 4x anti-alias tilt series
 newstack -shrink 2.0 ${Basename}_AliSB.st ${Basename}_AliSB_bin2.st

 newstack -shrink 4.0 ${Basename}_AliSB.st ${Basename}_AliSB_bin4.st

# transfer data to designated area if desired
 if [ $fullpath == "NA" ]
  then echo done no additional transfer
  else
 # transfer the data to a designated area and keep a copy in the current location
    if [ $Processing == 'y' ]
     then 
      mkdir ${fullpath}/${Basename}
     else
     echo "basename folder exits no need to generate"
     cp ${Basename}_AliSB.st ${fullpath}/${Basename}/
     cp ${Basename}_AliSB_bin2.st ${fullpath}/${Basename}/
     cp ${Basename}_AliSB_bin4.st ${fullpath}/${Basename}/
 # transfer the Processing folder to a designated area and keep a copy in the current location 
     cd $location
     cp -r ${Basename}_Processing ${fullpath}/${Basename}/
     echo done data transferred to ${fullpath}
    fi
fi

echo "Done!"
