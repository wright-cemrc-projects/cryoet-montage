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
      cp ${Basename}_*_0.0*.mrc ${Basename}_Processing/Tilt_0/
      cp ${Basename}_*_-0.0*.mrc ${Basename}_Processing/Tilt_0/
      cp ${Basename}_0.pl ${Basename}_Processing/Tilt_0/      
  else
      cp ${Basename}_*_${i}.0*.mrc ${Basename}_Processing/Tilt_${i}/
  # piece coordinate files needed for blendmont and also ready for subsequent inspection and Midas launching
      cp ${Basename}_${i}.pl ${Basename}_Processing/Tilt_${i}/
    fi;

  # generate a new stack per tilt for stitching (xxx_-6.mrc)  and stitch the stack in individual tilt folders called xxx_-6_blend.st using blendmont command
  # copy a stitched stack per tilt to the upper directory for whole montage tilt series generation

  newstack ${Basename}_Processing/Tilt_${i}/*.mrc ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.st

  # use blendmont command includes both edge function calculation and cross-correlation 
  blendmont -imin ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.st -imout ${Basename}_Processing/Tilt_${i}/${Basename}_${i}_blend.st -plin ${Basename}_Processing/Tilt_${i}/${Basename}_${i}.pl -roo ${Basename}_Processing/Tilt_${i}/${Basename}_${i} -sloppy
  cp ${Basename}_Processing/Tilt_${i}/${Basename}_${i}_blend.st ${Basename}_Processing/
  alterheader -del $pixelsize,$pixelsize,$pixelsize ${Basename}_Processing/${Basename}_${i}_blend.st
  # remane the piece coordinate files to match with the pre-blend new stack to avoid errors in Midas
  i=$(($i+$c))
 done
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
    if [ $answer == "y" ]
     then
       tif2mrc *.gain gain.mrc
      if [ $input == "K3" ]
       then
         for f in *.tif
          do
          motioncor2 -InTiff ${f} -OutMrc ${f/\.tif/\.mc.mrc} -Patch 5 5 -PixelSize $pixelsize -Gpu 0 1 2 3 -Gain $gain -RotGain $rotgain -FlipGain $flipgain
         done
      else
         example=$(ls *.eer | tail -1)
         header ${example} | grep 'Number of columns*' | awk '{print $9}' >> temp_1.txt
         frames=$(cat temp_1.txt)
         fractions=$(($frames/9))
         echo $fractions >> temp_2.txt
         cat ${example}.mdoc | grep 'FrameDosesAndNumber*' | awk '{print $4}' >> temp_3.txt
         paste temp_1.txt temp_2.txt temp_3.txt >> Intfile.txt
         for f in *.eer
          do 
          motioncor2 -InEER ${f} -OutMrc ${f/\.eer/\.oc.mrc} -EerSampling 2 -FmIntFile Intfile.txt -FtBin 2 -Patch 5 5 -Iter 10 -Tol 0.5 -Gpu 0 1 2 3 -Gain gain.mrc
          newstack -rotate 180  ${f/\.eer/\.oc.mrc} ${f/\.eer/\.mc.mrc}
         done
      fi
      rm temp*.txt 
      rm *.oc.mrc
    elif [ $answer == "SBgrid" ]
      then
       tif2mrc *.gain gain.mrc
      if [ $input == "K3" ]
       then
         for f in *.tif
          do
          MotionCor2 -InTiff ${f} -OutMrc ${f/\.tif/\.mc.mrc} -Patch 5 5 -PixelSize $pixelsize -Gpu 0 1 2 3 -Gain $gain -RotGain $rotg$
         done
      else
         example=$(ls *.eer | tail -1)
         header ${example} | grep 'Number of columns*' | awk '{print $9}' >> temp_1.txt
         frames=$(cat temp_1.txt)
         fractions=$(($frames/9))
         echo $fractions >> temp_2.txt
         cat ${example}.mdoc | grep 'FrameDosesAndNumber*' | awk '{print $4}' >> temp_3.txt
         paste temp_1.txt temp_2.txt temp_3.txt >> Intfile.txt
         for f in *.eer
          do
          motioncor2 -InEER ${f} -OutMrc ${f/\.eer/\.oc.mrc} -EerSampling 2 -FmIntFile Intfile.txt -FtBin 2 -Patch 5 5 -Iter 10 -Tol 0$
          newstack -rotate 180  ${f/\.eer/\.oc.mrc} ${f/\.eer/\.mc.mrc}
         done
      fi   
      rm temp*.txt
      rm *.oc.mrc
    else
       echo no motion correction
     fi
}

## main script start

echo "============main  process starting=============="
echo "put the bash script blendstitching_updated.sh and all piece coordinate files in the same folder where all frames in motion_corrected mrc format or unaligned tiff are"

 echo "starting tilt series index" 
 read folder_i
 echo "ending tilt series index"
 read folder_end
 echo "starting angle negative e.g. -60"
 read i
 echo "ending angle positive e.g. 60"
 read end
 echo "tilt increments e.g. 3"
 read c
 echo "Is motion correction needed (type y for MotionCor2/type SBgrid for motioncor2)"
 read answer
 echo "camera, type K3 for tif format collected using K3 or type F4 for eer format collected using Falcon4"
 read input
 echo "tiff only, rotation of the gain applied, 0 no rotation, 1 to 3 referes to 90, 180, 270 or NA for Falcon4"
 read rotgain
 echo "tiff only, flip gain applied, 0 no flipping, 1 flip upside down, 2 flip left and right or NA for Falcon4"
 read flipgain
 echo "unbinned pixel size"
 read pixelsize

location=$(pwd)
origin=$i

while [ $folder_i -le $folder_end ] 
do
  Basename=$(awk '{if(NR=='$folder_i') print $1}' folderlist.txt);
  cd $location/${Basename}/
# motion correction if needed 
 motioncorr

# remove old files
 for b in `ls *.oc.mrc`
 do
 if [[ -f "$b" ]];
  then 
    echo "old files need to be removed"
    rm *.oc.mrc
    echo "Done"
  else
    echo "no files need to be removed"
  fi
 done

 for a in `ls *.txt`;
  do 
  if [[ -f "$a" ]]
   then
    echo "old files need to be removed"
    rm $a
    echo "Done"
   else
    echo "no files need to be removed"
  fi
  done

# main functions start
 i=$origin
 tilts=$(($end-$i))
 tilts=$(($tilts/$c))
 tilts=$(($tilts+1))
 echo tilts = $tilts

# generate folder to hole all of the files
  mkdir ${Basename}_Processing

# perfom stitching function
 stitching;

 location2=$(pwd)

# go to the processing folder and generate the stitched tilt series 

 cd ${location2}/${Basename}_Processing/

 i=$origin

# generate a premature tiltlist file to hold the file names and used in function userlist to create final tiltlist.txt
 rm tiltlist.txt
 echo $tilts >> tiltlist.txt
 userlist;

# reset i for function usertilt
 i=$origin

# generate a list of tilt angles to insert into the stitched tilt series
  rm tilt.rawtlt
  usertilt;

# generate the stitched nxn unbinned tilt series
 newstack -tilt tilt.rawtlt -fileinlist tiltlist.txt -ou ${Basename}_AliSB.st
 
# generate a binned 2x and 4x anti-alias tilt series
 newstack -shrink 2.0 ${Basename}_AliSB.st ${Basename}_AliSB_bin2.st

 newstack -shrink 4.0 ${Basename}_AliSB.st ${Basename}_AliSB_bin4.st

echo $(pwd)
echo "Done"

folder_i=$(($folder_i+1));
cd ${location}
done

echo "Done!"
