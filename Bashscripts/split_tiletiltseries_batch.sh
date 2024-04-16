#!/bin.bash

##scripts for CryoMontage subtilt series generation developed by JaeYang from Wright Lab Oct.2021, edited 04_16_2024

echo CryoMontage subtilt series generation 

####### function ############

## generate a list of files to be used to form a new stack

Userlist() {
   while [ $i -le $end ]
    do
      if [[ $i == 0 ]]
        then  mv ${Basename}_*_-0.0*.mrc ${Basename}_subtilt_${tiltOne}_0.0.mrc
              mv ${Basename}_*_0.0*.mrc ${Basename}_subtilt_${tiltOne}_0.0.mrc
      else
        for j in $(seq -w $seqOne $period $seqLast)
        do 
          mv ${Basename}_${j}_${i}.0*mc.mrc ${Basename}_subtilt_${tiltOne}_${i}.0.mrc
         done
      fi
      echo ${Basename}_subtilt_${tiltOne}_${i}.0.mrc >> userlist.txt
      echo 0 >> userlist.txt
      i=$(($i+$c))
    done
}

## generate a list of tilt angles to be used to form a new stack

Usertilt() {
   while [ $i -le $end ]
     do
       echo $i >> user.rawtlt
       i=$(($i+$c))
   done
}

## align raw frames for motion correction

motioncorr() {
    if [ $answer == "SBgrid" ]
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
    elif [ $answer == "y" ]
      then
       tif2mrc *.gain gain.mrc
      if [ $input == "K3" ]
       then
         for f in *.tif
          do
          MotionCor2 -InTiff ${f} -OutMrc ${f/\.tif/\.mc.mrc} -Patch 5 5 -PixelSize $pixelsize -Gpu 0 1 2 3 -Gain $gain -RotGain $rotgain -FlipGain $flipgain
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
          Motioncor2 -InEER ${f} -OutMrc ${f/\.eer/\.oc.mrc} -EerSampling 2 -FmIntFile Intfile.txt -FtBin 2 -Patch 5 5 -Iter 10 -Tol 0.5 -Gpu 0 1 2 3 -Gain gain.mrc
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

echo "=================main processing starting=============="
echo "starting tilt series index"
read folder_i
echo "ending tilt series index"
read folder_end
echo "subtilt first (e.g.1)"
read tiltOne
echo "subtilt last (3x3 be 9)"
read tiltLast
echo "sequential starting number (000)"
read seqOne
echo "sequential ending number (000)"
read seqLast
#echo basename
#read Basename
echo "how many subtilts (3x3 be 9)"
read period
echo "starting angle negative (e.g. -60)"
read i
echo "ending angle positive (e.g. 60)"
read end
echo "tilt increment (e.g.2 or 3)"
read c
echo "Is motion correction needed (type y for MotionCor2/type SBgrid for motioncor2/type n for no correction)"
read answer
echo "camera, type K3 for tif format collected using K3 or type F4 for eer format collected using Falcon4"
read input
echo "tiff only, rotation of the gain applied, 0 no rotation, 1 to 3 referes to 90, 180, 270 or NA for Falcon4"
read rotgain
echo "tiff only, flip gain applied, 0 no flipping, 1 flip upside down, 2 flip left and right or NA for Falcon4"
read flipgain
echo "unbinned pixel size"
read pixelsize

echo "location to transfer data full path or NA"
read fullpath

location2=$(pwd)
origin=$i

while [ $folder_i -le $folder_end ] 
do
  Basename=$(awk '{if(NR=='$folder_i') print $1}' folderlist.txt);
  cd ${location2}/${Basename}/
 # calculate tilt numbers of a tilt 
 i=$origin
 tilts=$(($end-$i))
 tilts=$(($tilts/$c))
 tilts=$(($tilts+1))
 echo tilts = $tilts


 # motion correction if needed
 # remove old files
 for b in `ls *oc.mrc`
  do
  if [[ -f "$b" ]];
   then
    echo "old files need to be removed"
    rm *oc.mrc
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
 motioncorr
 # remove old files
 for b in `ls *oc.mrc`
  do
  if [ -f "$b" ];
  then
    echo "old files need to be removed"
    rm *oc.mrc
    echo "Done"
  else
    echo "no files need to be removed"
  fi
 done

 for a in `ls *.txt`;
  do
  if [ -f "$a" ]
   then
    echo "old files need to be removed"
    rm $a
    echo "Done"
   else
    echo "no files need to be removed"
  fi
 done

 # sorting and genarting subtilt from subtilt_1
 while [ $tiltOne -le $tiltLast ]
   do
    mkdir subTilt_${tiltOne}
    echo $tiltOne
     # each subtilt has tilt frames whose sequential numbers are periodically increasing by tile numbers/period value
     for a in $(seq -w $seqOne $period $seqLast)
       do
         # ignore the error messages 
         cp ${Basename}_${a}_*.*.mrc subTilt_${tiltOne}/
     done
    location=$(pwd)

    # go to each subtilt folder and generate the new stacks
    cd $location/subTilt_${tiltOne}/

    # reset the origin of i
    i=$origin

    # generate file list for newstack
    echo $tilts >> userlist.txt
    Userlist

    # reset the origin of i
    i=$origin

    # generate tilt angle list for newstack
    Usertilt

    # generate unbinned subtilt serires
    newstack -tilt user.rawtlt -fileinlist userlist.txt ${Basename}_subtilt_${tiltOne}_AliSB.st

    # Additional data transfer if desired
    if [ ${fullpath} == "NA" ]
     then echo no data transfer ${Basename}_subtilt_${tiltOne}_AliSB.st is generated
    else
     cp ${Basename}_subtilt_${tiltOne}_AliSB.st ${fullpath}
     echo done ${Basename}_subtilt_${tiltOne}_AliSB.st is transferred to ${fullpath}
    fi

    # go back to the upper folder to create the next subtilt
    cd $location/
    seqOne=$(($seqOne+1))
    seqLast=$(($seqLast+1))
    tiltOne=$(($tiltOne+1))
 done

folder_i=$(($folder_i+1));
cd ${location2}
done

echo DONE!

