    % ----------------------------------------------------------------------------
    %
    %                           function cirs2ecefiau06
    %
    %  this function trsnforms a vector from the cirs
    %    (gcrf), to an earth fixed (itrf) frame.  the results take into account
    %    the effects of  sidereal time, and polar motion.
    %
    %  author        : david vallado                  719-573-2600   2 may 2020
    %
    %  revisions
    %
    %  inputs          description                    range / units
    %    rcirs       - position vector cirs            km
    %    vcirs       - velocity vector cirs            km/s
    %    acirs       - acceleration vector cirs        km/s2
    %    ttt         - julian centuries of tt         centuries
    %    jdut1       - julian date of ut1             days from 4713 bc
    %    lod         - excess length of day           sec
    %    xp          - polar motion coefficient       arc sec
    %    yp          - polar motion coefficient       arc sec
    %    option      - which approach to use          a-2000a, b-2000b, c-2000xys
    %
    %  outputs       :
    %    recef       - position vector earth fixed    km
    %    vecef       - velocity vector earth fixed    km/s
    %    aecef       - acceleration vector earth fixedkm/s2
    %
    %  locals        :
    %    pm          - transformation matrix for itrf-pef
    %    st          - transformation matrix for pef-ire
    %
    %  coupling      :
    %   iau00pm      - rotation for polar motion      itrf-pef
    %   iau00era     - rotation for earth rotation    pef-ire
    %
    %  references    :
    %    vallado       2004, 205-219
    %
    % [recef,vecef,aecef] = cirs2ecefiau06  ( rcirs,vcirs,acirs,ttt,jdut1,lod,xp,yp,option, ddx, ddy );
    % ----------------------------------------------------------------------------

    function [recef,vecef,aecef] = cirs2ecefiau06( rcirs,vcirs,acirs,ttt,jdut1,lod,xp,yp,option)

    %      sethelp;

    % ---- ceo based, iau2000
    if option == 'c'
        [st]  = iau00era (jdut1 );
    end;

    % ---- class equinox based, 2000a
    if option == 'a'
        [gst,st] = iau00gst(jdut1, ttt, deltapsi, l, l1, f, d, omega, ...
            lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate);
    end;

    % ---- class equinox based, 2000b
    if option == 'b'
        [gst,st] = iau00gst(jdut1, ttt, deltapsi, l, l1, f, d, omega, ...
            lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate);
    end;

    [pm] = polarm(xp,yp,ttt,'06');

    % ---- setup parameters for velocity transformations
    thetasa= 7.29211514670698e-05 * (1.0  - lod/86400.0 );
    omegaearth = [0; 0; thetasa;];

    rpef  = st'*rcirs;
    recef = pm'*rpef;

    vpef  = st'*vcirs - cross( omegaearth,rpef );
    vecef = pm'*vpef;

    temp  = cross(omegaearth,rpef);
    aecef = pm'*(st'*acirs - cross(omegaearth,temp) - 2.0*cross(omegaearth,vpef));

