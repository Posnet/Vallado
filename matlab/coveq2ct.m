    % ----------------------------------------------------------------------------
    %
    %                           function coveq2ct
    %
    %  this function transforms a six by six covariance matrix expressed in
    %    equinoctial elements into one expressed in cartesian elements.
    %
    %  author        : david vallado                  719-573-2600   24 jul 2003
    %
    %  revisions
    %    vallado     - major update                                  26 aug 2015
    %
    %  inputs          description                    range / units
    %    eqcov       - 6x6 equinoctial covariance matrix
    %    eqstate     - 6x1 equinoctial orbit state    (a/n af ag chi psi lm/ln)
    %    anom        - anomaly                        'meana', 'truea', 'meann', 'truen'
    %    fr          - retrograde factor               +1, -1
    %
    %  outputs       :
    %    cartcov     - 6x6 cartesian covariance matrix
    %    tm          - transformation matrix
    %
    %  locals        :
    %    n           - mean motion                    rad
    %    af          - component of ecc vector
    %    ag          - component of ecc vector
    %    chi         - component of node vector in eqw
    %    psi         - component of node vector in eqw
    %    meanlon     - mean longitude                 rad
    %    nu          - true anomaly                   0.0  to 2pi rad
    %    m           - mean anomaly                   0.0  to 2pi rad
    %    r           - matrix of partial derivatives
    %    e0          - eccentric anomaly              0.0  to 2pi rad
    %
    %  coupling      :
    %    constastro
    %
    %  references    :
    %    Vallado and Alfano 2015
    %
    % [cartcov, tm] = coveq2ct(eqcov, eqstate, anom, fr);
    % ----------------------------------------------------------------------------

    function [cartcov, tm] = coveq2ct(eqcov, eqstate, anom, fr)

    % -------- define the gravitational constant
    constastro;

    % --------- determine which set of variables is in use ---------
    % -------- parse the orbit state
    if strcmp(anom,'truea') == 1 || strcmp(anom,'meana') == 1  % 1 is true
        a = eqstate(1) * 1000.0;  % in m
        n = sqrt(mum/a^3);  % mum 
    else
        if strcmp(anom,'truen') == 1 || strcmp(anom,'meann') == 1  % 1 is true
            n = eqstate(1);  % rad
            a = (mum / n^2)^(1/3);   % in m
        end
    end
    af      = eqstate(2);
    ag      = eqstate(3);
    chi     = eqstate(4);
    psi     = eqstate(5);
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        meanlonM = eqstate(6);
        omega = atan2( chi, psi);
        argp = atan2( ag,af ) - fr*atan2( chi,psi );
        ecc = sqrt (af^2 + ag^2);
        m = meanlonM - fr*omega - argp;
        [eccanom,nu] = newtonm ( ecc,m );
        meanlonNu = nu + fr*omega + argp;
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            meanlonNu = eqstate(6);
            omega = atan2( chi, psi);
            argp = atan2( ag,af ) - fr*atan2( chi,psi );
            nu = meanlonNu - fr*omega - argp;
            nu = rem (nu + twopi,twopi);
            ecc = sqrt (af^2 + ag^2);
            [eccanom,m] = newtonnu ( ecc,nu );
            meanlonM = fr*omega + argp + m;
            meanlonM = rem(meanlonM,2.0*pi);
        end
    end

    % needs to be mean longitude for eq2rv
    [reci, veci] = eq2rv( a/1000.0, af, ag, chi, psi, meanlonM, fr);
    rx = reci(1) * 1000.0;  % in m
    ry = reci(2) * 1000.0;
    rz = reci(3) * 1000.0;
    vx = veci(1) * 1000.0;
    vy = veci(2) * 1000.0;
    vz = veci(3) * 1000.0;

    magr = mag(reci)*1000; % m
    magv = mag(veci)*1000; % m/s

    A = n * a^2;
    B = sqrt(1.0 - ag^2 - af^2);
    C = 1.0 + chi^2 + psi^2;
    b = 1.0 / (1.0 + B);

    % same
    %G = n * a^2 * sqrt(1.0 - af^2 - ag^2);
    G = A*B;

    % -----------  initial guess -------------
    F0 = meanlonM;
    numiter = 25;
    ktr= 1;
    F1 = F0 - (F0 + ag*cos(F0) - af*sin(F0) - meanlonM)/(1.0 - ag*sin(F0) - af*cos(F0));
    while (( abs(F1-F0) > small ) && ( ktr <= numiter ))
        ktr = ktr + 1;
        F0= F1;
        F1 = F0 - (F0 + ag*cos(F0) - af*sin(F0) - meanlonM)/(1.0 - ag*sin(F0) - af*cos(F0));
    end;
    F = F1;

    X = a*((1.0 - ag^2 * b) * cos(F) + af*ag*b*sin(F) - af);
    Y = a*((1.0 - af^2 * b) * sin(F) + af*ag*b*cos(F) - ag);

    XD = n*a^2/magr * ( af*ag*b*cos(F) - (1.0 - ag^2*b)*sin(F) );
    YD = n*a^2/magr * ( (1.0 - af^2*b)*cos(F) - af*ag*b*sin(F) );
    
    % alt formulation, but needs L
    %XD = -n*a*(ag + sinL) / (sqrt(1.0 - af^2 - ag^2));
    %YD =  n*a*(af + cosL) / (sqrt(1.0 - af^2 - ag^2));

    % alt forms all are the same now
    %         sinL = ((1-af^2*b)*sin(F) + ag*af*b*cos(F) - ag) / (1 - ag*sin(F) - af*cos(F));
    %         cosL = ((1-ag^2*b)*cos(F) + ag*af*b*sin(F) - af) / (1 - ag*sin(F) - af*cos(F));
    %         XD = -n*a*(ag + sinL) / (B)
    %         YD =  n*a*(af + cosL) / (B)
    %         r = a*(1-af^2-ag^2) / (1 + ag*sinL + af*cosL);
    %         r = a*(1.0 - ag*sin(F) - af*cos(F));
    %         r = magr
    %         X = r*cosL;
    %         Y = r*sinL;
    
    % components of equinoctial system
    p0 = 1.0 / (1.0 + chi^2 + psi^2);
    fe = p0 * (1.0 - chi^2 + psi^2);  % 2nd one is minus???? no, + seems correct
    fq = p0 * 2.0 * chi * psi;
    fw = p0 * -2.0 * fr*chi;
    fvec = [fe, fq, fw];
    ge = p0 * 2.0 * fr*chi * psi;
    gq = p0 * fr*(1.0 + chi^2 - psi^2);
    gw = p0 * 2.0 * psi;
    gvec = [ge, gq, gw];
    we = p0 * 2.0 * chi;
    wq = p0 * -2.0 * psi;
    ww = p0 * fr*(1.0 - chi^2 - psi^2);
    wvec = [we, wq, ww];  
    
    partXaf =  ag*b*XD/n + a*Y*XD/G - a;
    partYaf =  ag*b*YD/n - a*X*XD/G;
    partXag = -af*b*XD/n + a*Y*YD/G;
    partYag = -af*b*YD/n - a*X*YD/G - a;
    
    partXDaf =  a*XD*YD / G - A/(magr^3) * ( a*ag*X/(1 + B) + X*Y/B );
    partYDaf = -a*XD^2 / G - A/(magr^3) * ( a*ag*Y/(1 + B) - X^2/B );
    partXDag =  a*YD^2 / G + A/(magr^3) * ( a*af*X/(1 + B) - Y^2/B );
    partYDag = -a*XD*YD / G + A/(magr^3) * ( a*af*Y/(1 + B) + X*Y/B );
  
    fprintf(1,'na %11.7f %11.7f  \n',n, a);
    fprintf(1,'XY %11.7f %11.7f %11.7f %11.7f\n',X, Y, XD, YD);
    fprintf(1,'ABC %11.7f %11.7f %11.7f \n',A, B, C);
    fprintf(1,'part %11.7f %11.7f %11.7f %11.7f \n', partXDaf, partYDaf, partXDag, partYDag);

    dMdnu = (1.0 - ecc^2)^1.5 / ( (1.0 + ecc*cos(nu))^2 );  % dm/dv

    % ---------------- calculate matrix elements ------------------
    % ---- partials of (rx ry rz vx vy vz) wrt a
    if strcmp(anom,'truea') == 1 || strcmp(anom,'meana') == 1  % 1 is true
        p0 = 1.0 / a;
        p1 = -1.0 / (2.0*a);
    else
        if strcmp(anom,'truen') == 1 || strcmp(anom,'meann') == 1  % 1 is true
            p0 = -2.0/(3.0*n);
            p1 =  1.0/(3.0*n);
        end
    end

    tm(1,1) =  p0 * rx;
    tm(2,1) =  p0 * ry;
    tm(3,1) =  p0 * rz;
    tm(4,1) =  p1 * vx;
    tm(5,1) =  p1 * vy;
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        tm(6,1) =  p1 * vz;
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            %                 cosl = cos(meanlonNu);
            %                 sinl = sin(meanlonNu);
            %                 ch = a*(2.0*af*ag*cosl + 2.0*ag + (ag^2 - af^2 + 1.0)*sinl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 ck = a*(2.0*af*ag*sinl + 2.0*af + (af^2 - ag^2 + 1.0)*cosl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 dhkv = 1.0*(chi*cosl - psi*sinl);
            %
            %                 tm(1:3,2) = dXdh * fe    + dYdh * ge;
            %                 sensh = (ch*dhkv(1) - tm(1,2))/tm(1,6);
            %                 tm(:,2) = tm(:,2) + tm(:,6)*sensh;
            %
            %                 tm(1:3,3) = dXdk * fe    + dYdk * ge;
            %                 sensk = (ck * dhkVec(1) - tm(1,3))/tm(1,6)
            %                 tm(:,3) = tm(:,3) + tm(:,6)*sensk
            %
            tm(6,1) =  p1 * vz;
            %tm(6,1) = dMdnu * tm(6,1);
            %tm(6,1) = 0.0;
        end
    end

    % ---- partials of (rx ry rz vx vy vz) wrt af
    tm(1,2) = partXaf * fe + partYaf * ge;
    tm(2,2) = partXaf * fq + partYaf * gq;
    tm(3,2) = partXaf * fw + partYaf * gw;
    tm(4,2) = partXDaf * fe + partYDaf * ge;
    tm(5,2) = partXDaf * fq + partYDaf * gq;
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        tm(6,2) = partXDaf * fw + partYDaf * gw;
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            %                 cosl = cos(meanlonNu);
            %                 sinl = sin(meanlonNu);
            %                 ch = a*(2.0*af*ag*cosl + 2.0*ag + (ag^2 - af^2 + 1.0)*sinl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 ck = a*(2.0*af*ag*sinl + 2.0*af + (af^2 - ag^2 + 1.0)*cosl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 dhkv = 1.0*(chi*cosl - psi*sinl);
            %
            %                 tm(1:3,2) = dXdh * fq    + dYdh * gq;
            %                 sensh = (ch*dhkv(1) - tm(1,2))/tm(1,6);
            %                 tm(:,2) = tm(:,2) + tm(:,6)*sensh;
            %
            %                 tm(1:3,3) = dXdk * fq    + dYdk * gq;
            %                 sensk = (ck * dhkVec(1) - tm(1,3))/tm(1,6)
            %                 tm(:,3) = tm(:,3) + tm(:,6)*sensk
            %
            tm(6,2) = partXDaf * fw + partYDaf * gw;
            %tm(6,2) = dMdnu * tm(6,2);
            %tm(6,2) = 0.0;
        end
    end

    % ---- partials of (rx ry rz vx vy vz) wrt ag
    tm(1,3) = partXag * fe + partYag * ge;
    tm(2,3) = partXag * fq + partYag * gq;
    tm(3,3) = partXag * fw + partYag * gw;
    tm(4,3) = partXDag * fe + partYDag * ge;
    tm(5,3) = partXDag * fq + partYDag * gq;
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        tm(6,3) = partXDag * fw + partYDag * gw;
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            %                 cosl = cos(meanlonNu);
            %                 sinl = sin(meanlonNu);
            %                 ch = a*(2.0*af*ag*cosl + 2.0*ag + (ag^2 - af^2 + 1.0)*sinl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 ck = a*(2.0*af*ag*sinl + 2.0*af + (af^2 - ag^2 + 1.0)*cosl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 dhkv = 1.0*(chi*cosl - psi*sinl);
            %
            %                 tm(1:3,2) = dXdh * fw    + dYdh * gw;
            %                 sensh = (ch*dhkv(1) - tm(1,2))/tm(1,6);
            %                 tm(:,2) = tm(:,2) + tm(:,6)*sensh;
            %
            %                 tm(1:3,3) = dXdk * fw    + dYdk * gw;
            %                 sensk = (ck * dhkVec(1) - tm(1,3))/tm(1,6)
            %                 tm(:,3) = tm(:,3) + tm(:,6)*sensk
            %
            tm(6,3) = partXDag * fw + partYDag * gw;
            %tm(6,3) = dMdnu * tm(6,3);
            %tm(6,3) = 0.0;
        end
    end

    % ---- partials of (rx ry rz vx vy vz) wrt chi
    p0 = 2.0 * fr / C;
    tm(1,4) = p0 * (psi * (Y*fe - X*ge) - X*we);
    tm(2,4) = p0 * (psi * (Y*fq - X*gq) - X*wq);
    tm(3,4) = p0 * (psi * (Y*fw - X*gw) - X*ww);
    tm(4,4) = p0 * (psi * (YD*fe - XD*ge) - XD*we);
    tm(5,4) = p0 * (psi * (YD*fq - XD*gq) - XD*wq);
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        tm(6,4) = p0 * (psi * (YD*fw - XD*gw) - XD*ww);
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            %                 cosl = cos(meanlonNu);
            %                 sinl = sin(meanlonNu);
            %                 ch = a*(2.0*af*ag*cosl + 2.0*ag + (ag^2 - af^2 + 1.0)*sinl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 ck = a*(2.0*af*ag*sinl + 2.0*af + (af^2 - ag^2 + 1.0)*cosl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 dhkv = 1.0*(chi*cosl - psi*sinl);
            %
            %                 tm(4:6,2) = dXdotdh * fe + dYdotdh * ge;
            %                 sensh = (ch*dhkv(1) - tm(1,2))/tm(1,6);
            %                 tm(:,2) = tm(:,2) + tm(:,6)*sensh;
            %
            %                 tm(4:6,3) = dXdotdk * fe + dYdotdk * ge;
            %                 sensk = (ck * dhkVec(1) - tm(1,3))/tm(1,6)
            %                 tm(:,3) = tm(:,3) + tm(:,6)*sensk
            %
            tm(6,4) = p0 * (psi * (YD*fw - XD*gw) - XD*ww);
            %tm(6,4) = dMdnu * tm(6,4);
            %tm(6,4) = 0.0;
        end
    end

    % ---- partials of (rx ry rz vx vy vz) wrt psi 
    p0 = 2.0 * fr / C;
    tm(1,5) = p0 * (chi * (X*ge - Y*fe) + Y*we);
    tm(2,5) = p0 * (chi * (X*gq - Y*fq) + Y*wq);
    tm(3,5) = p0 * (chi * (X*gw - Y*fw) + Y*ww);
    tm(4,5) = p0 * (chi * (XD*ge - YD*fe) + YD*we);
    tm(5,5) = p0 * (chi * (XD*gq - YD*fq) + YD*wq);
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        tm(6,5) = p0 * (chi * (XD*gw - YD*fw) + YD*ww);
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            %                 cosl = cos(meanlonNu);
            %                 sinl = sin(meanlonNu);
            %                 ch = a*(2.0*af*ag*cosl + 2.0*ag + (ag^2 - af^2 + 1.0)*sinl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 ck = a*(2.0*af*ag*sinl + 2.0*af + (af^2 - ag^2 + 1.0)*cosl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 dhkv = 1.0*(chi*cosl - psi*sinl);
            %
            %                 tm(4:6,2) = dXdotdh * fq + dYdotdh * gq;
            %                 sensh = (ch*dhkv(1) - tm(1,2))/tm(1,6);
            %                 tm(:,2) = tm(:,2) + tm(:,6)*sensh;
            %
            %                 tm(4:6,3) = dXdotdk * fq + dYdotdk * gq;
            %                 sensk = (ck * dhkVec(1) - tm(1,3))/tm(1,6)
            %                 tm(:,3) = tm(:,3) + tm(:,6)*sensk
            %
            tm(6,5) = p0 * (chi * (XD*gw - YD*fw) + YD*ww);
            %tm(6,5) = dMdnu * tm(6,5);
            %tm(6,5) = 0.0;
        end
    end

    % ---- partials of (rx ry rz vx vy vz) wrt meanlon
    p0 = 1.0 / n;
    p1 = - n * a^3 / magr^3;
    tm(1,6) =  p0 * vx;
    tm(2,6) =  p0 * vy;
    tm(3,6) =  p0 * vz;
    tm(4,6) =  p1 * rx;
    tm(5,6) =  p1 * ry;
    if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
        tm(6,6) =  p1 * rz;
    else
        if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
            %                 cosl = cos(meanlonNu);
            %                 sinl = sin(meanlonNu);
            %                 ch = a*(2.0*af*ag*cosl + 2.0*ag + (ag^2 - af^2 + 1.0)*sinl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 ck = a*(2.0*af*ag*sinl + 2.0*af + (af^2 - ag^2 + 1.0)*cosl) / (C*(1.0 + af*cosl + ag*sinl)^2);
            %                 dhkv = 1.0*(chi*cosl - psi*sinl);
            %
            %                 tm(4:6,2) = dXdotdh * fw + dYdotdh * gw;
            %                 sensh = (ch*dhkv(1) - tm(1,2))/tm(1,6);
            %                 tm(:,2) = tm(:,2) + tm(:,6)*sensh;
            %
            %                 tm(4:6,3) = dXdotdk * fw + dYdotdk * gw;
            %                 sensk = (ck * dhkVec(1) - tm(1,3))/tm(1,6)
            %                 tm(:,3) = tm(:,3) + tm(:,6)*sensk
            %
            tm(6,6) =  p1 * rz;
            %tm(6,6) = dMdnu * tm(6,4);
            %tm(6,6) = 0.0;

            % similar to ct2cl true...
%             r_dot_v = sqrt(rx*vx + ry*vy + rz*vz);
%             ecc_term = magv*magv - mum/magr;
%             ecc_x = (ecc_term*rx - r_dot_v*vx)/mum;
%             ecc_y = (ecc_term*ry - r_dot_v*vy)/mum;
%             ecc_z = (ecc_term*rz - r_dot_v*vz)/mum;
%             r_dot_e = sqrt(rx*ecc_x + ry*ecc_y + rz*ecc_z);
%             nu_scale = -sign(r_dot_v)/sqrt(1-cos(nu)*cos(nu));
%             magr3 = magr^3;
%             temp = ry*(vx*vy - mum*rx*ry/magr3) - rx*ecc_term + rz*(vx*vz - mum*rx*rz/magr3);
%             temp = temp - rx*(vy*vy + vz*vz - mum/magr + mum*rx*rx/magr3) + vx*r_dot_v;
%             temp = -temp/(mum*magr*ecc) - rx*r_dot_e/(magr3*ecc) - tm(2,1)*r_dot_e/(magr*ecc*ecc);
%             tm(6,1) = temp*nu_scale;
%             temp = rx*(vx*vy - mum*rx*ry/magr3) - ry*ecc_term + rz*(vy*vz - mum*ry*rz/magr3);
%             temp = temp-ry*(vx*vx + vz*vz - mum/magr + mum*ry*ry/magr3) + vy*r_dot_v;
%             temp = -temp/(mum*magr*ecc) - ry*r_dot_e/(magr3*ecc) - tm(2,2)*r_dot_e/(magr*ecc*ecc);
%             tm(6,2) = temp*nu_scale;
%             temp = rx*(vx*vz - mum*rx*rz/magr3) - rz*ecc_term + ry*(vy*vz - mum*ry*rz/magr3);
%             temp = temp - rz*(vx*vx + vy*vy - mum/magr + mum*rz*rz/magr3) + vz*r_dot_v;
%             temp = -temp/(mum*magr*ecc) - rz*r_dot_e/(magr3*ecc) - tm(2,3)*r_dot_e/(magr*ecc*ecc);
%             tm(6,3) = temp*nu_scale;
%             temp = ry*(rx*vy - 2*ry*vx) + rx*(ry*vy + rz*vz) + rz*(rx*vz - 2*rz*vx);
%             temp = -temp/(mum*magr*ecc) - tm(2,4)*r_dot_e/(magr*ecc*ecc);
%             tm(6,4) = temp*nu_scale;
%             temp = rx*(ry*vx - 2*rx*vy) + ry*(rx*vx + rz*vz) + rz*(ry*vz - 2*rz*vy);
%             temp = -temp/(mum*magr*ecc) - tm(2,5)*r_dot_e/(magr*ecc*ecc);
%             tm(6,5) = temp*nu_scale;
%             temp = rz*(rx*vx + ry*vy) + rx*(rz*vx - 2*rx*vz) + ry*(rz*vy - 2*ry*vz);
%             temp = -temp/(mum*magr*ecc) - tm(2,6)*r_dot_e/(magr*ecc*ecc);
%             tm(6,6) = temp*nu_scale;
        end
    end

    % ---------- calculate the output covariance matrix -----------
    [cartcov] =  tm * eqcov * tm';

