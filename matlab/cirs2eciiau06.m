    % ----------------------------------------------------------------------------
    %
    %                           function cirs2eciiau06
    %
    %  this function transforms a vector from the cirs frame, to
    %    the eci mean equator mean equinox (gcrf).
    %
    %  author        : david vallado                  719-573-2600   2 may 2020
    %
    %  revisions
    %
    %  inputs          description                    range / units
    %    rcirs       - position vector earth fixed    km
    %    vcirs       - velocity vector earth fixed    km/s
    %    acirs       - acceleration vector earth fixedkm/s2
    %    ttt         - julian centuries of tt         centuries
    %    option      - which approach to use          a-2000a, b-2000b, c-2000xys
    %    ddx         - eop correction for x           rad
    %    ddy         - eop correction for y           rad
    %
    %  outputs       :
    %    reci        - position vector eci            km
    %    veci        - velocity vector eci            km/s
    %    aeci        - acceleration vector eci        km/s2
    %
    %  locals        :
    %    nut         - transformation matrix for ire-gcrf
    %
    %  coupling      :
    %   iau00era     - rotation for earth rotyation   pef-ire
    %   iau00xys     - rotation for prec/nut          ire-gcrf

    %
    %  references    :
    %    vallado       2004, 205-219
    %
    % [reci,veci,aeci] = cirs2eciiau06 ( rcirs,vcirs,acirs,ttt,option, ddx, ddy );
    % ----------------------------------------------------------------------------

    function [reci,veci,aeci] = cirs2eciiau06 ( rcirs,vcirs,acirs,ttt,option, ddx, ddy );

    sethelp;

    % ---- ceo based, iau2006
    if option == 'c'
        [x,y,s,pnb] = iau06xys (ttt, ddx, ddy);
    end;

    % ---- class equinox based, 2000a
    if option == 'a'
        [ deltapsi, pnb, prec, nut, l, l1, f, d, omega, ...
            lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate ...
            ] = iau06pna (ttt);
    end;

    % ---- class equinox based, 2000b
    if option == 'b'
        [ deltapsi, pnb, prec, nut, l, l1, f, d, omega, ...
            lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate ...
            ] = iau06pnb (ttt);
    end;

    % ---- perform transformations
    reci = pnb*rcirs;
    veci = pnb*vcirs;
    aeci = pnb*acirs;

