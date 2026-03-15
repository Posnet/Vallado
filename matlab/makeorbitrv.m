%
% ------------------------------------------------------------------------------
%
%                           function makeorbitrv
%
%  this function propagtes an orbit around for one rev using either kepler
%  or pkepler (secular j2). 
%
%  author        : david vallado                  719-573-2600    14 mar 2006
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
%
%  coupling      :
%    kepler      - find r and v at future time

%
%  references    :
%    vallado       2004, 
%
% [x,y,z] = makeorbitrv ( 2455545.0, 'j', [5003.400903511, -3817.812007872, 4720.200666830],[5.489294908, 3.005055561, -3.39013016]);
% ------------------------------------------------------------------------------

function [x,y,z] = makeorbitrv(jd, kind, reci, veci);

    % -------------------------  implementation   -------------------------
    rad = 180.0/pi;
    error = 'ok';
    if (kind == 'k')
        outfile = fopen('makeorbrvKep.out', 'wt');   
    else
        if (kind == 'p')
        outfile = fopen('makeorbrvJ2.out', 'wt');   
    else
        if (kind == 'j')
           outfile = fopen('makeorbrvJ4.out', 'wt');   
        end
        end
    end
           
      
    % --------------- approximate ast with gst for this simple demo --------------
%    gmst= gstime( jd );
%    st(1,1) =  cos(gmst);
%    st(1,2) = -sin(gmst);
%    st(1,3) =  0.0;
%    st(2,1) =  sin(gmst);
%    st(2,2) =  cos(gmst);
%    st(2,3) =  0.0;
%    st(3,1) =  0.0;
%    st(3,2) =  0.0;
%    st(3,3) =  1.0;    
%    recef = st'*reci; 
        
%    fprintf(1,'st gmst %11.7f ast %11.7f ome  %11.7f \n',gmst*180/pi,ast*180/pi,omegaearth*180/pi );

    x(1,1) = reci(1);     
    y(1,1) = reci(2); 
    z(1,1) = reci(3); 

    ndot = 0.0;
    nddot = 0.0;
%    period = 2.0*pi*sqrt(a*a*a/398600.4418);
    jdut1 = jd;
    
    fprintf(outfile,' 1601   xx \n');
    fprintf(outfile,'Time (UTCG)                x (km)             y (km)             z (km)         vx (km/sec)     vy (km/sec)     vz (km/sec)\n');
    fprintf(outfile,'-------------------------    ---------------    ---------------    ---------------    ------------    ------------    ------------\n');

    for i = 0:100
%        dtsec = (i-1)*period/120.0;
        dtsec = i*960.0;   % 30.0sec, 240 every 4 mins 
        jdut1 = jd + dtsec/86400.0;
        % ----------- propogate satellite forward by time ----------------
        if (kind=='k')
            [reci1, veci1] = kepler ( reci, veci, dtsec);
        end
        if (kind=='p')
            [reci1, veci1] = pkepler( reci, veci, dtsec, ndot, nddot );
        end
        if (kind=='j')
            [reci1, veci1] = pkeplerj4( reci, veci, dtsec, ndot, nddot );
        end

%        fprintf(1,'reci1 %4i x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f \n',i,reci1,veci1 );
%        gmst= gstime( jd+dtsec/86400.0 );
%        st(1,1) =  cos(gmst);
%        st(1,2) = -sin(gmst);
%        st(1,3) =  0.0;
%        st(2,1) =  sin(gmst);
%        st(2,2) =  cos(gmst);
%        st(2,3) =  0.0;
%        st(3,1) =  0.0;
%        st(3,2) =  0.0;
%        st(3,3) =  1.0;    
%        recef = st'*reci1';

 
%         [latgc,latgd,lon,hellp] = ijk2ll ( reci1, jdut1 );
%         if lon < 0.0
%             lon=lon + 2*pi;
%         end;
%        fprintf(outfile,'%11.7f  %11.7f   1.0 \n',latgd*rad,lon*rad );  
           [h, m, s] = sec2hms(dtsec);
        fprintf(outfile,'1 Jun 2018 %2i:%2i:%7.4f  %16.9f  %16.9f %16.9f  %16.9f %16.9f  %16.9f \n',h, m, s, reci1, veci1 );  
        
%        x(i,1) = reci1(1);     
%        y(i,1) = reci1(2); 
%        z(i,1) = reci1(3); 
        
     end;  % for
    
reci
dtsec
