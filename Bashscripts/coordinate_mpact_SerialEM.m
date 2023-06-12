
% function to generate coordinate files for each tilt and coordiante the
% shift with Y  * cosine of tilt angle
% coordinate_mpact_SerialEM_K3(number of picecs in x,number of picecs in y,xOverlap,yOverlap,tilt starting angle
% (negative),ending angle (positive),tilt increment, basename)
function coordinateTableCell = coordinate_mpact_SerialEM(x,y,xOverlap,yOverlap,imageX,imageY,varargin)
inputx = x;
inputy = y;
pieceX = 0;
incrementX = imageX - xOverlap;
pieceY = 0;
incrementY = imageY - yOverlap;
rows = x * y;
tiltscheme = (varargin{1}:varargin{3}:varargin{2}); 
basename = varargin{4};
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
 for i = 1:y
    x = i * inputx;
   for ii = origin:x
    coordinateTable(ii,:) = [pieceX pieceY 0];
    if (mod(i,2) == 0) && (ii ~= x)
     pieceX = pieceX - incrementX;
    elseif (mode(i,2)~= 0) && (ii ~= x)
     pieceX = pieceX + incrementX;
    end
   end
 origin = origin + inputx;
 pieceY = pieceY + round(incrementY * cosd(a));
 end
 output{count} = sprintf('%s_%d.txt',basename,a);
 coordinateTableCell{count} = coordinateTable;
 writematrix(coordinateTableCell{count},output{count},'Delimiter','\t');
 count = count + 1;
end




