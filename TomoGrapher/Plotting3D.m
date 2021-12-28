% PLOTTING3D(T,tiltsD,size,max) plots voxel positions x-y and z as the
% normalized dose against non-offsets collection in 3D.
% The color of each point corresponds to the value of the normalized dose.
%
% INPUT T is the csv file name.
%
% INPUT tiltsD is the approximate mean of the total dose of a regular tilt
% series with 1 e/A per tilt. e.g. 60 to -60, 3 degree increment, tiltsD =
% 41, if 2 degree increment, tiltsD = 61.
%
% INPUT size(optional) defines the size of the scattering points,defautl as
% 2.
%
% INPUT max(optional) defines the maximum number on the legend z-axis.

function csvF = Plotting3D(T,tiltsD,size,max)
if nargin < 1
    error('specify the csv file from Tomographer')
end

if nargin == 1
    size = 2;
    tiltsD = 41;
end

csvF = readtable(T);
csvF = table2array(csvF);
x = csvF(:,1);
y = csvF(:,3);
z = csvF(:,4);
doseN = z/tiltsD;
figure(1)
scatter3(x(:),y(:),doseN(:),size,doseN(:),'filled')

if nargin == 4
   figure(1);
   lim = caxis;
   caxis([1 max]) 
end
end
