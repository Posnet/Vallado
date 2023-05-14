% ------------------------------------------------------------------------------
%
%                           function tradec2rv
%
%  this function converts range, topcentric right acension, declination, and rates
%    into geocentric equatorial (eci) position and velocity vectors.  
%
%  author        : david vallado           davallado@gmail.com    4 nov 2022
%
%  revisions
%
%  inputs          description                              range / units
%    rho         - satellite range from site                km
%    trtasc      - topocentric right ascension              0.0 to 2pi rad
%    tdecl       - topocentric declination                  -pi/2 to pi/2 rad
%    drho        - range rate                               km/s
%    dtrtasc     - topocentric rtasc rate                   rad / s
%    dtdecl      - topocentric decl rate                    rad / s
%    rseci       - eci site position vector                 km
%    lod         - excess length of day                     sec
%
%  outputs       :
%    reci        - eci position vector                      km
%    veci        - eci velocity vector                      km/s
%
%  locals        :
%    rhov        - eci range vector from site               km
%    drhov       - eci velocity vector from site            km / s
%    omegaearth  - eci earth's rotation rate vec            rad / s
%    tempvec     - temporary vector
%    latgc       - site geocentric latitude                 rad
%
%  coupling      :
%    mag         - magnitude of a vector
%    rot3        - rotation about the 3rd axis
%    rot2        - rotation about the 2nd axis
%
%  references    :
%    vallado       2022, 254, eq 4-1 to 4-2
%
% [reci, veci] = tradec2rv (rho, trtasc, tdecl, drho, dtrtasc, dtdecl, rseci, lod)
% ------------------------------------------------------------------------------

function [reci, veci] = tradec2rv (rho, trtasc, tdecl, drho, dtrtasc, dtdecl, rseci, lod)
    constmath;

    % --------------------- implementation ------------------------
    latgc = asin(rseci(3) / mag(rseci));
    
    thetasa= earthrot * (1.0  - lod/86400.0 );
    omegaearth = [0.0; 0.0; thetasa];
    cross(omegaearth, rseci, vseci);

    % --------  calculate topocentric slant range vectors ------------------
    rhov(1) = rho * cos(tdecl) * cos(trtasc);
    rhov(2) = rho * cos(tdecl) * sin(trtasc);
    rhov(3) = rho * sin(tdecl);

    drhov(1) = drho * cos(tdecl) * cos(trtasc) - ...
        rho * sin(tdecl) * cos(trtasc) * dtdecl - ...
        rho * cos(tdecl) * sin(trtasc) * dtrtasc;
    drhov(2) = drho * cos(tdecl) * sin(trtasc) - ...
        rho * sin(tdecl) * sin(trtasc) * dtdecl + ...
        rho * cos(tdecl) * cos(trtasc) * dtrtasc;
    drhov(3) = drho * sin(tdecl) + rho * cos(tdecl) * dtdecl;

    % ------ find eci range vector from site to satellite ------
    reci = rhov + rseci;
    veci = drhov + cos(latgc) * vseci;
    
    end  % tradec2rv

