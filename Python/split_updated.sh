#!/bin.bash

##scripts for CryoMontage subtilt series generation developed by JaeYang from Wright Lab Oct.2021, edited 12_2_2021

echo CryoMontage subtilt series generation 

####### function ############

## generate a list of files to be used to form a new stack

Userlist() {
   while [ $i -le $end ]
    do
      if [[ $i == 0 ]]
        then  mv ${Basename}_*_-0.0.*.mrc ${Basename}_subtilt_${tiltOne}_0.0.mrc
              mv ${Basename}_*_0.0.*.mrc ${Basename}_subtilt_${tiltOne}_0.0.mrc
      else
        for j in $(seq -w $seqOne $period $seqLast)
        do 
          mv ${Basename}_${j}_${i}.0.mc.mrc ${Basename}_subtilt_${tiltOne}_${i}.0.mrc
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
       motioncor2 -InTiff ${f} -OutMrc ${f}.mc.mrc -Patch 5 5 -PixelSize $pixelsize -Gpu 0 1 2 3 -Gain $gain -RotGain $rotgain -$
     else
       echo no motion correction
     fi
}

## main script start

echo "=================main processing starting=============="
echo "subtilt first (e.g.1)"
read tiltOne
echo "subtilt last (3x3 be 9)"
read tiltLast
echo "sequential starting number (000)"
read seqOne
echo "sequential ending number (000)"
read seqLast
echo basename
read Basename
echo "how many subtilts (3x3 be 9)"
read period
echo "starting angle negative (e.g. -60)"
read i
echo "ending angle positive (e.g. 60)"
read end
echo "tilt increment (e.g.2 or 3)"
read c
echo "location to transfer data full path or NA"
read fullpath

origin=$i


# calculate tilt numbers of a tilt 
tilts=$(($end/$c))
tilts=$(($tilts*2+1))
echo tilts = $tilts

# motion correction if needed
motioncorr

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

    # additional data transfer if desired
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

echo DONE!

