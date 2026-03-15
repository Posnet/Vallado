function pn = legPoly(x,i)
% LEGPOLY calculates the legendre polynomial of i-th order in x
% x can be a scalar, a vector or a matrix
 
% pn : nth legendre polynomial
% pn_1 : n-1 legendre polynomial
% pn_p1 : n+1 legendre polynomial
 
% Konstantinos G.
% Modified version of the C code in "Numerical recipes in C"
 
pn_p1 = 1;
pn = ones(size(x,1), size(x,2));
 
if(i>0)
 pn_1 = x.*pn;
 if(i==1)
 pn = pn_1;
 else
 for i = 2 : i
 pn_p1 = (x .* (2 * i - 1).*pn_1 - (i - 1)*pn)/i;
 pn = pn_1;
 pn_1 = pn_p1;
 end
 pn = pn_p1;
 end
end
end
