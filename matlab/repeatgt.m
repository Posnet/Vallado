% ------------------------------------------------------------------------------
%
%                           function repeatgt
%
%  these subroutine calculates repeat ground tracks
%
%  author        : david vallado                  719-573-2600   10 feb 2006
%
%  revisions
%
%  inputs          description                    range / units
%    r           - ecef position vector           km
%
%  outputs       :
%    latgc       - geocentric latitude            -pi to pi rad
%
%  locals        :
%    temp        - diff between geocentric/
%
%  coupling      :
%
%  references    :
%    vallado       2001, 174-179, alg 12 and alg 13, ex 3-3
%
% [a] = repeatgt ( b );
% ------------------------------------------------------------------------------

function [a] = repeatgt ( b );

        twopi   =  2.0*pi;
        small   =  0.00000001;      % small value for tolerances
        re      =  6378.137;
        j2      =  0.00108263       % earth j2
        oearth  =  7.292115146706979e-5; % omega earth, rad /sec
        gm      = 3.986004418;

        rad = 180/pi;
        
        a = 6400.0;
        ecc = 0.0001;
        incl = 98.00/rad;

        nanom = sqrt( gm/(a*a*a) );
        nanom
        
%        nnodal = 1;
        
        p = a*(1.0-ecc*ecc);
        
        % -------------------------  implementation   -----------------
        raanrate = -1.5*j2*nanom*cos(incl)/(p*p);

        revs2rep = 107;
        revspday = 16.0;
        
        days2rpt = revs2rep/revspday;
        days2rpt
        
        lonshift = days2rpt*2*pi/revs2rep;
        lonshift*rad
        
        nnodal = oearth*revspday;
        nnodal

        % old a422 way
        rp = 160.0;
        period2b = 2.0*pi*sqrt( gm/(a*a*a) );
        period2b
        
        periodnew = period2b + period2b*raanrate/oearth;
        periodnew 
        
        anew = (gm*(periodnew/oearth)^2)^(1.0/3.0);
        anew
        
        enew = 1-rp/anew;
        enew
        
        
        
        pause;
        
        % ------------- iterate to find geodetic latitude -------------
        i= 1;
        olddelta = latgd + 10.0;

        while ((abs(olddelta-latgd)>=small) & (i<10))
            olddelta= latgd;
            sintemp = sin( latgd );
            c       = re  / (sqrt( 1.0 -eesqrd*sintemp*sintemp ));
            latgd= atan( (r(3)+c*eesqrd*sintemp)/temp );
            i = i + 1;
          end
