%
% ------------------------------------------------------------------------------
%
%                           function target
%
%  this function accomplishes the targeting problem using kepler/pkepler &
%    lambert.
%
%  author        : david vallado                  719-573-2600    1 mar 2001
%
%  inputs          description                    range / units
%    rint        - initial position vector of int km
%    vint        - initial velocity vector of int km/s
%    rtgt        - initial position vector of tgt km
%    vtgt        - initial velocity vector of tgt km/s
%    dm          - direction of motion for gauss  'l','s'
%    kind        - type of propagator             'k','p'
%    dtsec       - time of flight to the int      s
%
%  outputs       :
%    v1t         - initial transfer velocity vec  km/s
%    v2t         - final transfer velocity vec    km/s
%    dv1         - initial change velocity vec    km/s
%    dv2         - final change velocity vec      km/s
%    error       - error flag from gauss          'ok', ...
%
%  locals        :
%    transnormal - cross product of trans orbit   km
%    intnormal   - cross product of int orbit     km
%    r1tgt       - position vector after dt, tgt  km
%    v1tgt       - velocity vector after dt, tgt  km/s
%    rirt        - rint(4) * r1tgt(4)
%    cosdeltanu  - cosine of deltanu              rad
%    sindeltanu  - sine of deltanu                rad
%    deltanu     - angl between position vectors rad
%    i           - index
%
%  coupling      :
%    kepler      - find r and v at future time
%    lambertu - find velocity vectors at each end of transfer
%    lncom2      - linear combination of two vectors and constants
%
%  references    :
%    vallado       2001, 468-474, alg 58
%
% [v1t,v2t,dv1,dv2] = target ( rint,vint,rtgt,vtgt, dm,kind, dtsec,ndot,nddot );
% ------------------------------------------------------------------------------

function [v1t,v2t,dv1,dv2] = target ( rint,vint,rtgt,vtgt, dm,kind, dtsec,ndot,nddot );

% -------------------------  implementation   -------------------------
        error = 'ok';

        % ----------- propogate target forward by time ----------------
        if (kind=='k')
            [r1tgt,v1tgt] = kepler ( rtgt,vtgt,0.0 );
          end
        if (kind=='p')
            [r1tgt,v1tgt] = pkepler( rtgt,vtgt,dtsec,ndot, nddot );
          end

        % ----------- calculate transfer orbit between r's ------------
        if ( error == 'ok' )  
            [v1t,v2t,error1] = lambertu( rint,r1tgt,dm,'n',dtsec );

            if ( error1 == '      ok' )
                dv1 = -vint + v1t;
                dv2 =  v1tgt - v2t;
            else
                fprintf(1, ' err %s ',error1 );
                for i= 1 : 3
                    v1t(i)= 0.0;
                    v2t(i)= 0.0;
                    dv1(i)= 0.0;
                    dv2(i)= 0.0;
                  end
              end
          end

