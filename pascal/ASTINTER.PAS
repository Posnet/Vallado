(*     ----------------------------------------------------------------      *)

                               UNIT ASTINTER;

(*   this file contains fundamental astrodynamic procedures and functions
*   relating to interplanetary calculations.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2004
*                              by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*              30 may 07  david vallado
*                           3rd edition baseline
*    changes :
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
*                                                                           
*      ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    Uses
       AstMath,
       AstTime,
       Ast2body;


PROCEDURE PlanetRV           ( PlanetNum                             : INTEGER;
                               WhichEpoch,WhichCoord                 : Str3;
                               JD                                    : EXTENDED;
                               VAR R,V                               : Vector   );

PROCEDURE Interplanetary     ( R1,R2,Rbo,Rimpact,Mu1,Mut,Mu2         : EXTENDED;
                               VAR Deltav1,Deltav2,Vbo,Vretro        : EXTENDED );


{ ------------------------- Constants used in this Library ---------------------
|
|     TwoPi      : EXTENDED =     6.28318530717959;
|     Rad        : EXTENDED =    57.29577951308230;
|
 ----------------------------------------------------------------------------- }

(*     -----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     -----------------------------------------------------------------      *)
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE PLANETRV
|
|  This PROCEDURE calculate the planetary ephemerides using the Epoch J2000.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|                                               
|  Inputs          Description                    Range / Units
|    PlanetNum   - Number of planet               1..9
|    JD          - Julian Date                    days from 4713 BC
|
|  OutPuts       :
|    R           - XYZ position vector            AU
|    V           - XYZ velocity vector            AU / day
|
|  Locals        :
|    ArgLat      -
|    TrueLon     -
|    LonPer      -
|    TUUT1       - Julian cenuries from Jan 1, 2000
|    TU2         - Tu squared
|    TU3         - TU Cubed
|    N           -
|    obliquity   - angle between ecliptic and
|                    Earth equator                rad
|    a           - Semi or axis
|    ecc         - eccentricity
|    p           - semi-parameter
|    incl        - inclination
|    omega       - ecliptic long of ascending node
|    argp        - ecliptic arg of perihelion
|    nu          - ecliptic true anomaly
|    m           - ecliptic mean anomaly
|    LLong       - True longitude
|    LongP       - longitude of perihelion
|    e0          -
|
|  Coupling      :
|    RealMOD     -
|    LnCom1      -
|    NewtonM     -
|    RandV       -
|    ROT1        -
|
|  References    :
|    Vallado       2007, 995
|
 --------------------------------------------------------------------------- }

PROCEDURE PlanetRV           ( PlanetNum                             : INTEGER;
                               WhichEpoch,WhichCoord                 : Str3;
                               JD                                    : EXTENDED;
                               VAR R,V                               : Vector   );
   CONST
     TwoPi      : EXTENDED =     6.28318530717959;
     Rad        : EXTENDED =    57.29577951308230;
   VAR
     TUDaySun,ArgLat,TrueLon,LonPer,Tut1,tut12,Tut13,n,eps, a,ecc,p,incl,omega,argp,nu,
     llong,longp,m,e0       : EXTENDED;
     i       : INTEGER;
     Re,Ve : Vector;
   BEGIN
     { ----------------- Approximate TTDB with TUT1 ----------------- }
     Tut1 := ( JD - 2451545.0 ) / 36525.0;
     Tut12:= Tut1*Tut1;
     Tut13:= Tut12*Tut1;
{}
     IF WhichEpoch = 'J20' THEN
       BEGIN
         { ---------- Mean equinox of J2000 in degrees (XYZ) -------- }
         CASE PlanetNum OF
           1: BEGIN  { -----------Mercury ---------}
                a    :=   0.387098310;
                ecc  :=   0.20563175 + 0.000020406*Tut1 - 0.0000000284*Tut12 -0.00000000017*Tut13;
                incl :=   7.004986   - 0.0059516*Tut1   + 0.00000081*Tut12 + 0.000000041*Tut13;
                LongP:=  77.456119   + 0.1588643*Tut1   - 0.00001343*Tut12 + 0.000000039*Tut13;
                Omega:=  48.330893   - 0.1254229*Tut1   - 0.00008833*Tut12 - 0.000000196*Tut13;
                LLong:= 252.250906   + 149472.6746358*Tut1 - 0.00000535*Tut12  + 0.000000002*Tut13;
              END;
           2: BEGIN  { -----------Venus  ----------}
                a    :=   0.723329820;
                ecc  :=   0.00677188 - 0.000047766*Tut1 + 0.0000000975*Tut12 + 0.00000000044*Tut13;
                incl :=   3.394662 - 0.0008568*Tut1 - 0.00003244*Tut12  + 0.000000010*Tut13;
                LongP:= 131.563707 + 0.0048646*Tut1 - 0.00138232*Tut12 - 0.000005332*Tut13;
                Omega:=  76.679920 - 0.2780080*Tut1 - 0.00014256*Tut12  - 0.000000198*Tut13;
                LLong:= 181.979801 + 58517.8156760*Tut1 + 0.00000165*Tut12 - 0.000000002*Tut13;
              END;
           3: BEGIN  { -----------Earth  ----------}
                a    :=   1.000001018;
                ecc  :=   0.01670862 - 0.000042037*Tut1 - 0.0000001236*Tut12 + 0.00000000004*Tut13;
                incl :=   0.0000000 + 0.0130546*Tut1 - 0.00000931*Tut12  - 0.000000034*Tut13;
                LongP:= 102.937348 + 0.3225557*Tut1 + 0.00015026*Tut12  + 0.000000478*Tut13;
                Omega:=  0.0;
                LLong:= 100.466449 + 35999.3728519*Tut1 - 0.00000568*Tut12 + 0.000000000*Tut13;
              END;
           4: BEGIN  { -----------Mars   ----------}
                a    :=   1.523679342;
                ecc  :=   0.09340062 + 0.000090483*Tut1 - 0.0000000806*Tut12  - 0.00000000035*Tut13;
                incl :=   1.849726 - 0.0081479*Tut1 - 0.00002255*Tut12  - 0.000000027*Tut13;
                LongP:= 336.060234 + 0.4438898*Tut1 - 0.00017321*Tut12 + 0.000000300*Tut13;
                Omega:=  49.558093 - 0.2949846*Tut1 - 0.00063993*Tut12 - 0.000002143*Tut13;
                LLong:= 355.433275 + 19140.2993313*Tut1 + 0.00000261*Tut12 - 0.000000003*Tut13;
              END;
           5: BEGIN  { -----------Jupiter ---------}
                a    :=   5.202603191  + 0.0000001913*Tut1;
                ecc  :=   0.04849485 + 0.000163244*Tut1 - 0.0000004719*Tut12 - 0.00000000197*Tut13;
                incl :=   1.303270 - 0.0019872*Tut1 + 0.00003318*Tut12  + 0.000000092*Tut13;
                LongP:=  14.331309 + 0.2155525*Tut1 + 0.00072252*Tut12 - 0.000004590*Tut13;
                Omega:= 100.464441 + 0.1766828*Tut1 + 0.00090387*Tut12 - 0.000007032*Tut13;
                LLong:=  34.351484 + 3034.9056746*Tut1 - 0.00008501*Tut12  + 0.000000004*Tut13;
              END;
           6: BEGIN  { -----------Saturn  ---------}
                a    :=   9.554909596 - 0.0000021389*Tut1;
                ecc  :=   0.05550862 - 0.000346818*Tut1 - 0.0000006456*Tut12 + 0.00000000338*Tut13;
                incl :=   2.488878 + 0.0025515*Tut1 - 0.00004903*Tut12 + 0.000000018*Tut13;
                LongP:=  93.056787 + 0.5665496*Tut1 + 0.00052809*Tut12 + 0.000004882*Tut13;
                Omega:= 113.665524 - 0.2566649*Tut1 - 0.00018345*Tut12 + 0.000000357*Tut13;
                LLong:=  50.077471 + 1222.1137943*Tut1 + 0.00021004*Tut12 - 0.000000019*Tut13;
              END;
           7: BEGIN  { -----------Uranus  ---------}
                a    :=  19.218446062 - 0.0000000372*Tut1 + 0.00000000098*Tut12;
                ecc  :=   0.04629590 - 0.000027337*Tut1 + 0.0000000790*Tut12 + 0.00000000025*Tut13;
                incl :=   0.773196 - 0.0016869*Tut1 + 0.00000349*Tut12 + 0.000000016*Tut13;
                LongP:= 173.005159 + 0.0893206*Tut1 - 0.00009470*Tut12 + 0.000000413*Tut13;
                Omega:=  74.005947 + 0.0741461*Tut1 + 0.00040540*Tut12 + 0.000000104*Tut13;
                LLong:= 314.055005 + 428.4669983*Tut1 - 0.00000486*Tut12  + 0.000000006*Tut13;
              END;
           8: BEGIN  { -----------Neptune ---------}
                a    :=  30.110386869 - 0.0000001663*Tut1 + 0.00000000069*Tut12;
                ecc  :=   0.00898809 + 0.000006408*Tut1 - 0.0000000008*Tut12;
                incl :=   1.769952 + 0.0002257*Tut1 + 0.00000023*Tut12  - 0.000000000*Tut13;
                LongP:=  48.123691 + 0.0291587*Tut1 + 0.00007051*Tut12 - 0.000000000*Tut13;
                Omega:= 131.784057 - 0.0061651*Tut1 - 0.00000219*Tut12 - 0.000000078*Tut13;
                LLong:= 304.348665 + 218.4862002*Tut1 + 0.00000059*Tut12 - 0.000000002*Tut13;
              END;
           9: BEGIN  { -----------Pluto  ----------}
                a    :=  39.53758;
                ecc  :=   0.250877;
                incl :=  17.13233;
                LongP:= 224.6148;
                Omega:= 110.4065;
                LLong:= 218.88735;
              END;
         END;  { Case }
       END;
{}
     IF WhichEpoch = 'ODA' THEN
       BEGIN
         { ------------ Mean equinox of date in degrees (XYZ) ----------- }
         CASE PlanetNum OF
           1: BEGIN  { -----------Mercury ---------}
                a    :=   0.387098310;
                ecc  :=   0.20563175 +    0.000020406*Tut1 - 0.0000000284*Tut12 - 0.00000000017*Tut13;
                incl :=   7.004986 +      0.0018215*Tut1 - 0.00001809*Tut12 + 0.000000053*Tut13;
                LongP:=  77.456119 +      1.5564775*Tut1 + 0.00029589*Tut12 + 0.000000056*Tut13;
                Omega:=  48.330893 +      1.1861890*Tut1 + 0.00017587*Tut12 + 0.000000211*Tut13;
                LLong:= 252.250906 + 149474.0722491*Tut1 + 0.00030397*Tut12  + 0.000000018*Tut13;
              END;
           2: BEGIN  { -----------Venus  ----------}
                a    :=   0.723329820;
                ecc  :=   0.00677188 -   0.000047766*Tut1 + 0.0000000975*TUT12 + 0.00000000044*TUT13;
                incl :=   3.394662 +     0.0010037*Tut1 - 0.00000088*Tut12  - 0.000000007*Tut13;
                LongP:= 131.563707 +     1.4022188*Tut1 - 0.00107337*Tut12 - 0.000005315*Tut13;
                Omega:=  76.679920 +     0.9011190*Tut1 + 0.00040665*Tut12  - 0.000000080*Tut13;
                LLong:= 181.979801 + 58519.2130302*Tut1 + 0.00031060*Tut12 + 0.000000015*Tut13;
              END;
           3: BEGIN  { -----------Earth  ----------}
                a    :=   1.000001018;
                ecc  :=   0.01670862 -   0.000042037*Tut1 - 0.0000001236*TUT12 + 0.00000000004*TUT13;
                incl :=   0.0;
                LongP:= 102.937348 +     1.7195269*Tut1 + 0.00045962*Tut12  + 0.000000499*TUT13;
                LLong:= 100.466449 + 36000.7698231*Tut1 + 0.00030368*Tut12 + 0.000000021*TUT13;
              END;
           4: BEGIN  { -----------Mars   ----------}
                a    :=   1.523679342;
                ecc  :=   0.09340062 +   0.000090483*Tut1 - 0.0000000806*Tut12 - 0.00000000035*TUT13;
                incl :=   1.849726 -     0.0006010*Tut1 + 0.00001276*Tut12  - 0.000000006*TUT13;
                LongP:= 336.060234 +     1.8410331*Tut1 + 0.00013515*Tut12 + 0.000000318*TUT13;
                Omega:=  49.558093 +     0.7720923*Tut1 + 0.00001605*Tut12 + 0.000002325*TUT13;
                LLong:= 355.433275 + 19141.6964746*Tut1 + 0.00031097*Tut12 + 0.000000015*TUT13;
              END;
           5: BEGIN  { -----------Jupiter ---------}
                a    :=   5.202603191 + 0.0000001913*TUT1;
                ecc  :=   0.04849485 +  0.000163244*Tut1 - 0.0000004719*Tut12 - 0.00000000197*TUT13;
                incl :=   1.303270 -    0.0054966*Tut1 + 0.00000465*Tut12  - 0.000000004*TUT13;
                LongP:=  14.331309 +    1.6126668*Tut1 + 0.00103127*Tut12 - 0.000004569*TUT13;
                Omega:= 100.464441 +    1.0209550*Tut1 + 0.00040117*Tut12 + 0.000000569*TUT13;
                LLong:=  34.351484 + 3036.3027889*Tut1 + 0.00022374*Tut12  + 0.000000025*TUT13;
              END;
           6: BEGIN  { -----------Saturn  ---------}
                a    :=   9.554909596 - 0.0000021389*Tut1;
                ecc  :=   0.05550862 -  0.000346818*Tut1 - 0.0000006456*Tut12 + 0.00000000338*TUT13;
                incl :=   2.488878 -    0.0037363*Tut1 - 0.00001516*Tut12 + 0.000000089*TUT13;
                LongP:=  93.056787 +    1.9637694*Tut1 + 0.00083757*Tut12 + 0.000004899*Tut13;
                Omega:= 113.665524 +    0.8770979*Tut1 - 0.00012067*Tut12 - 0.000002380*Tut13;
                LLong:=  50.077471 + 1223.5110141*Tut1 + 0.00051952*Tut12 - 0.000000003*TUT13;
              END;
           7: BEGIN  { -----------Uranus  ---------}
                a    :=  19.218446062 - 0.0000000372*Tut1  + 0.00000000098*TUT12;
                ecc  :=   0.04629590 -  0.000027337*Tut1 + 0.0000000790*Tut12 + 0.00000000025*TUT13;
                incl :=   0.773196 +   0.0007744*Tut1 + 0.00003749*Tut12 - 0.000000092*TUT13;
                LongP:= 173.005159 +   1.4863784*Tut1 + 0.00021450*Tut12 + 0.000000433*TUT13;
                Omega:=  74.005947 +   0.5211258*Tut1 + 0.00133982*Tut12 + 0.000018516*Tut13;
                LLong:= 314.055005 + 429.8640561*Tut1 + 0.00030434*Tut12  + 0.000000026*TUT13;
              END;
           8: BEGIN  { -----------Neptune ---------}
                a    :=  30.110386869 - 0.0000001663*Tut1 + 0.00000000069*TUT12;
                ecc  :=   0.00898809 +  0.000006408*Tut1 -  0.0000000008*TUT12;
                incl :=   1.769952 -   0.0093082*Tut1 - 0.00000708*Tut12  + 0.000000028*TUT13;
                LongP:=  48.123691 +   1.4262677*Tut1 + 0.00037918*Tut12 - 0.000000003*TUT13;
                Omega:= 131.784057 +   1.1022057*Tut1 + 0.00026006*Tut12 - 0.000000636*TUT13;
                LLong:= 304.348665 + 219.8833092*Tut1 + 0.00030926*Tut12 + 0.000000018*TUT13;
              END;
           9: BEGIN  { -----------Pluto  ----------}
                a    :=  39.53758;
                ecc  :=   0.250877;
                incl :=  17.13233;
                LongP:= 224.6148;
                Omega:= 110.4065;
                LLong:= 218.88735;
              END;
         END;  { Case }
       END;
{}
     incl := incl / Rad;  { Convert to radians }
     LongP:= LongP / Rad;
     Omega:= Omega / Rad;
     LLong:= LLong / Rad;

     LLong:= REALMOD( LLong,TwoPI );
     LongP:= REALMOD( LongP,TwoPI );
     Omega:= REALMOD( Omega,TwoPI );
     Argp := LongP - Omega;
     M    := LLong - LongP;

     NewTonM( ecc,M,  E0,Nu );

     { ------------- Find Heliocentric ecliptic r and v ------------- }
     p       := a*(1.0-ecc*ecc);
     ArgLat  := Argp+Nu;
     TrueLon := Omega+Argp+Nu;
     LonPer  := Omega+Argp+Pi;
     RANDV( P,ecc,Incl,Omega,Argp,Nu,ArgLat,TrueLon,LonPer, R,V );

     { ---- Correct the velocity because we used TTdb - days!! ------ }
     TUDaySun:= 1.0/58.1324409;  { 1.0 / days per sun TU}
     LNCOM1( TUDaySun,V,  V );

     IF WhichCoord = 'GEO' THEN
       BEGIN
         { ------------ Find obliquity of the ecliptic angle -------- }
         Eps := 23.439291 - 0.0130042*TUT1 - 0.000000164*TUT12
                + 0.000000504*TUT13;
         Eps := RealMOD( Eps,360.0 );
         Eps := Eps / Rad;

         { -------------- Rotate to Geocentric coordinates ---------- }
         ROT1( R ,-eps, R );
         ROT1( V ,-eps, V );
       END;

     IF Show = 'S' THEN
       BEGIN
         WriteLn( FileOut,'New Case, tut1 =',tut1:11:7,' Planet ',PlanetNum:3,
                          WhichCoord:5,' ',WhichEpoch:3,'----------- ' );
         WriteLn( FileOut,'r helio ',r[1]:11:6,r[2]:11:6,r[3]:11:6,r[4]:11:6 );
         WriteLn( FileOut,v[1]:11:6,v[2]:11:6,v[3]:11:6,v[4]:11:6 );
         WriteLn( FileOut,r[1]*149597870.0:11:1,r[2]*149597870.0:11:1,
                          r[3]*149597870.0:11:1,r[4]*149597870.0:11:1,'km' );
         WriteLn( FileOut,v[1]*29.784691674:11:6,v[2]*29.784691674:11:6,
                          v[3]*29.784691674:11:6,v[4]*29.784691674:11:6 );
       END;
     IF Show = 'Y' THEN
         WriteLn( WhichCoord:5,a:10:6,Ecc:13:6,Incl*rad:10:6,Omega*rad:12:6,LongP*rad:14:8,
                  LLong*rad:14:5,Argp*rad:12:6,M*rad:12:6,Nu*rad:12:6 );
   END;  { PROCEDURE PlanetRV }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE INTERPLANETARY
|
|  This PROCEDURE calculates the delta v's for an interplanetary mission using a
|    patched conic approximation.  The transfer assumes circular orbits for each
|    of the planets.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units        Ex Value
|    R1          - Radius of planet 1 from sun    km
|    R2          - Radius of planet 2 from sun    km
|    Rbo         - Radius at burnout at planet 1  km
|    Rimpact     - Radius at impact on planet 2   km
|    Mu1         - Grav parameter of planet 1     km3/s2
|    Mut         - Grav parameter of planet Sun   km3/s2
|    Mu2         - Grav parameter of planet 2     km3/s2
|
|  OutPuts       :
|    DeltaV1     - Hyperb Exc vel at planet 1 SOI km/s
|    DeltaV2     - Hyperb Exc vel at planet 2 SOI km/s
|    Vbo         - Burnout vel at planet 1        km/s
|    Vretro      - Retro vel at surface planet 2  km/s
|
|  Locals        :
|    SME1        - Spec Mech Energy of 1st orbit  Km2/s
|    SMEt        - Spec Mech Energy of trans orbitKm2/s
|    SME2        - Spec Mech Energy of 2nd orbit  Km2/s
|    Vcs1        - Vel of 1st orbit at dv 1 point Km/s
|    Vcs2        - Vel of 2nd orbit at dv 2 point Km/s
|    Vt1         - Vel of Trans orbit at dv 1 pnt Km/s
|    Vt2         - Vel of Trans orbit at dv 2 pnt Km/s
|    A           - Semimajor Axis of Trans orbit  Km
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 47-48
|
 --------------------------------------------------------------------------- }

PROCEDURE Interplanetary     ( R1,R2,Rbo,Rimpact,Mu1,Mut,Mu2         : EXTENDED;
                               VAR Deltav1,Deltav2,Vbo,Vretro        : EXTENDED );
   VAR
     SME1,SME2,SMEt, Vcs1, Vcs2, Vt1, Vt2, A,TP       : EXTENDED;
   BEGIN
     { - Find a, SME, apogee and perigee velocities of trans orbit -- }
     A   := (R1+R2) * 0.5;
     SMEt:= -Mut/ (2.0*A);
     Vt1 := SQRT( 2.0*( (Mut/R1) + SMEt ) );
     Vt2 := SQRT( 2.0*( (Mut/R2) + SMEt ) );

     { ---- Find circular velocities of launch and target planet ---- }
     Vcs1:= SQRT( Mut/R1 );
     Vcs2:= SQRT( Mut/R2 );

     { ---- Find delta velocities for Hohmann transfer portion  ----- }
     DeltaV1:= ABS( Vt1 - Vcs1 );
     DeltaV2:= ABS( Vcs2 - Vt2 );

     { - Find SME and burnout/impact vel of launch / target planets - }
     SME1  := Deltav1*DeltaV1 * 0.5;
     SME2  := Deltav2*DeltaV2 * 0.5;
     Vbo   := SQRT( 2.0*( (Mu1/Rbo) + SME1 ) );
     Vretro:= SQRT( 2.0*( (Mu2/Rimpact) + SME2 ) );

     IF Show IN ['Y','S'] THEN
       BEGIN
         TP:= Pi*SQRT( a*a*a/Mut );  { Transfer Period in secs }
         WriteLn( FileOut,'     Transfer Period = ',TP/3.1536E07:8:3,' yrs  or  ',TP/86400.0:8:3,' days' );
         Writeln( FileOut );
         WriteLn( FileOut,'Vcs  km/s':19,vcs1:9:4,' ':10,vcs2:9:4);
         WriteLn( FileOut,' Vt  km/s':19,vt1:9:4,' ':10,vt2:9:4 );
         WriteLn( FileOut,'SME km2/s2':19,SME1:9:4,' ',SMEt:9:3,SME2:9:4 );
       END;

   END;  { PROCEDURE Interplanetary }

BEGIN

END.  { Unit AstInter }
{}
