
% function to generate coordinate txt files for each tilt with implemention 
% of the shift adjustment in Y  * cosine of tilt angle;
% coordinate_mpact_SerialEM4_1(x,y,xOverlap,yOverlap,imageX, imageY, 
% tilt starting angle(zero),first ending angle (positive),tilt increment, 
% basename, pretilt);
% x represents montage size in x, e.g. 3 in 3x3 ; y represents montage size
% in y, 3 in 3x3;
% imageX refers to the full frame size in x; imageY refers to the full
% frame size in y, e.g. 4096 as imageX and imageY for a square Falcon
% camera;
% pretilt refers to the angle of a pre-tilted lamella;
% all sizes are represented in pixels.

function coordinateTableCell = coordinate_mpact_SerialEM4_1(x,y,xOverlap,yOverlap,imageX,imageY,varargin)
inputx = x;
inputy = y;
pieceX = 0;
incrementX = imageX - xOverlap;
pieceY = 0;
incrementY = imageY - yOverlap;
rows = x * y;
tiltscheme = (varargin{1}:varargin{3}:varargin{2}); 
basename = varargin{4};
pretilt = varargin{5};
count = 1;
for a = tiltscheme(1:end)
i = 1;
y = inputy;
x = inputx;
origin = 1;
incrementX = imageX - xOverlap;
pieceY = 0;
incrementY = imageY - yOverlap;
rows = inputx * inputy;
pieceX = 0;
coordinateTable = zeros(rows,3);
angle = a - pretilt;
 for i = 1:x
    y = i * inputy;
   for ii = origin:y
     coordinateTable(ii,:) = [pieceX pieceY 0];
     pieceY = pieceY + round(incrementY * cosd(angle));
   end
 origin = origin + inputx;
 pieceX = pieceX + incrementX;
 pieceY = 0;
 end
 output{count} = sprintf('%s_%d.txt',basename,a);
 outputPL{count} = sprintf('%s_%d.pl',basename,a);
 coordinateTableCell{count} = coordinateTable;
 writematrix(coordinateTableCell{count},output{count},'Delimiter','\t');
 movefile(output{count},outputPL{count});
 count = count + 1;
end

    



