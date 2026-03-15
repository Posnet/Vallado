%     -----------------------------------------------------------------
%
%                              Ex11_2.m
%
%  this file demonstrates example 11-2.
%
%                          companion code for
%             fundamentals of astrodynamics and applications
%                                 2013
%                            by david vallado
%
%     (w) 719-573-2600, email dvallado@agi.com
%
%     *****************************************************************
%
%  current :
%             4 nov 15  david vallado
%                         original
%  changes :
%             4 nov 15  david vallado
%                         original baseline
%
%     *****************************************************************

     constmath;
     constastro;
     j2 = 0.00108263;
     rate= (360.0/365.2421897)/rad/86164.0989036973 % {rad/sec }
     % intermediate conversions
     360.0/365.2421897
     (360.0/365.2421897)/rad
     rate*tusec
     reca = 1.0 / 3.5;

     % ---- ex 11-2 ---- }
     % 800 km altitude 
     a = 7178.1363;
     ecc= 0.0;
     incl = acos(-a^3.5*2*rate*(1.0-ecc^2)^2/(3.0*re^2*j2*sqrt(mu)));        
     fprintf(1,'i %11.5f a %11.5f  e %11.7f %11.7f  %11.7f \n', incl*rad, a, ecc, a*(1.0+ecc)-re, a*(1.0-ecc)-re);

     % change ecc
     incl= 98.627/rad;
     ecc= 0.02;
     temp= -1.5*j2*cos(incl)*re^2*sqrt(mu)/(rate*(1.0-ecc*ecc)^2);
     a= temp^reca;
     fprintf(1,'i %11.5f a %11.5f  e %11.7f %11.7f  %11.7f \n', incl*rad, a, ecc, a*(1.0+ecc)-re, a*(1.0-ecc)-re);
     
     % now verify alternate equations
     a = 7179.82095;
     ecc = 0.02;
     incl = acos(-a^3.5*2*rate*(1.0-ecc^2)^2/(3.0*re^2*j2*sqrt(mu)));        
     incl * rad   

     a = 7179.82095;
     incl = 98.627/rad;
     ecc = sqrt(1.0 - sqrt(-3.0*re^2*j2*sqrt(mu)*cos(incl)/(2.0*a^3.5*rate)));        
     ecc   

     