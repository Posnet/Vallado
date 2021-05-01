% ------------------------------------------------------------------------------
%
%                           function rvs2raz
%
%  this function converts range, azimuth, and elevation values with slant
%    range and velocity vectors for a satellite from a radar site in the
%    topocentric horizon (sez) system.
%
%  author        : david vallado                  719-573-2600   22 jun 2002
%
%  revisions
%                -
%
%  inputs          description                    range / units
%    rhovec      - sez satellite range vector     km
%    drhovec     - sez satellite velocity vector  km / s
%
%  outputs       :
%    rho         - satellite range from site      km
%    az          - azimuth                        0.0 to 2pi rad
%    el          - elevation                      -pi/2 to pi/2 rad
%    drho        - range rate                     km / s
%    daz         - azimuth rate                   rad / s
%    del         - elevation rate                 rad / s
%
%  locals        :
%    sinel       - variable for sin( el )
%    cosel       - variable for cos( el )
%    sinaz       - variable for sin( az )
%    cosaz       - variable for cos( az )
%    temp        -
%    temp1       -
%
%  coupling      :
%    mag         - magnitude of a vector
%
%  references    :
%    vallado       2001, 250-251, eq 4-4, eq 4-5
%
% [rho,az,el,drho,daz,del] = rvs2raz ( rhosez,drhosez );
% ------------------------------------------------------------------------------

function [rho,az,el,drho,daz,del] = rvs2raz ( rhosez,drhosez );

        halfpi       =    pi*0.5;

        % -------------------------  implementation   -----------------
        small        =    0.00000001;

        % ------------- calculate azimuth and elevation ---------------
        temp= sqrt( rhosez(1)*rhosez(1) + rhosez(2)*rhosez(2) );
        if ( abs( rhosez(2) ) < small )
            if ( temp < small )
                az   =  atan2( drhosez(2) , -drhosez(1) );
              else
                if ( rhosez(1) > 0.0 )
                    az= pi;
                  else
                    az= 0.0;
                  end
              end
          else
            az= atan2( rhosez(2) , -rhosez(1) );
          end

        if ( ( temp < small ) )     % directly over the north pole
            el= sign(rhosez(3))*halfpi; % +- 90
          else
            el= asin( rhosez(3) / rhosez(4) );
          end

        % -------  calculate range, azimuth and elevation rates -------
        drho= dot(rhosez,drhosez)/rho;
        if ( abs( temp*temp ) > small )
            daz= ( drhosez(1)*rhosez(2) - drhosez(2)*rhosez(1) ) / ( temp*temp );
          else
            daz= 0.0;
          end

        if ( abs( temp ) > small )
            del= ( drhosez(3) - drho*sin( el ) ) / temp;
          else
            del= 0.0;
          end

