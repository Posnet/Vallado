%
% ----------------------------------------------------------------------------
%
%                           function covo22ct
%
%  this function transforms a six by six covariance matrix expressed in the
%    orbit plane (ntw) into one expressed in cartesian
%
%  author        : david vallado                  719-573-2600   17 jul 2003
%
%  revisions
%    vallado     - send out tm                                   25 jul 2003
%
%  inputs          description                    range / units
%    covopntw    - 6x6 orbit plane ntw covariance matrix
%    cartstate   - 6x1 cartesian orbit state      (x y z vx vy vz)
%
%  outputs       :
%    cartcov     - 6x6 cartesian covariance matrix
%
%  locals        :
%    r           - position vector                m
%    v           - velocity vector                m/s
%    tm          - transformation matrix
%    temv        - temporary vector
%
%  coupling      :
%    none
%
%  references    :
%    none
%
%  [cartcov,tm] = covo22ct( covopntw,cartstate );
% ----------------------------------------------------------------------------

function [cartcov,tm] = covo22ct( covopntw,cartstate );

        x  = cartstate(1);
        y  = cartstate(2);
        z  = cartstate(3);
        vx = cartstate(4);
        vy = cartstate(5);
        vz = cartstate(6);
        r = [x;y;z];
        v = [vx;vy;vz];

        tv = unit(v);
        temv = cross(r,v);
        wv = unit(temv);
        nv = cross(tv,wv);

        for i = 1:6
            for j = 1:6
                tm(i,j) = 0.0;
              end;
         end;

        tm(1,1) = nv(1);
        tm(1,2) = nv(2);
        tm(1,3) = nv(3);
        tm(2,1) = tv(1);
        tm(2,2) = tv(2);
        tm(2,3) = tv(3);
        tm(3,1) = wv(1);
        tm(3,2) = wv(2);
        tm(3,3) = wv(3);

        tm(4,4) = nv(1);
        tm(4,5) = nv(2);
        tm(4,6) = nv(3);
        tm(5,4) = tv(1);
        tm(5,5) = tv(2);
        tm(5,6) = tv(3);
        tm(6,4) = wv(1);
        tm(6,5) = wv(2);
        tm(6,6) = wv(3);

        tm = tm';

        cartcov = tm * covopntw * tm';


