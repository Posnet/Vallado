(*     ----------------------------------------------------------------      *)

                               UNIT ASTPERT;

(*   this file contains fundamental astrodynamic procedures and functions
*   allowing the calcualtions of perturbations. most of these routines
*   are discussed in ch 8.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2007
*                              by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*              10 jan 08  david vallado
*                           fix RE in drag of pertaccel
*              30 may 07  david vallado
*                           3rd printing baseline
*    changes :
*              21 jul 05  david vallado
*                           2nd printing baseline
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
*                                                                           
*      ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    USES
        AstMath,
        AstTime,
        AstIod,
        Ast2body;

    VAR C,S : Matrix;

{}

PROCEDURE PKEPLER            ( Ro,Vo                                 : Vector;
                               ndot,nddot,DtTU                       : EXTENDED;
                               VAR R,V                               : Vector   );

PROCEDURE J2DragPert         ( Incl,Ecc,N,NDot                       : EXTENDED;
                               VAR OmegaDOT,ArgpDOT,EDOT             : EXTENDED );

PROCEDURE PREDICT            ( JD,latgd,LST                          : EXTENDED;
                               r,v,rs                                : Vector;
                               WhichKind                             : CHAR;
                               VAR Rho,Az,El,tRtasc,tDecl            : EXTENDED;
                               VAR Vis                               : Str11    );

PROCEDURE InitGravityField   ( Order                                 : INTEGER;
                               VAR C,S                               : Matrix   );

PROCEDURE LegPoly            ( Latgc                                 : EXTENDED;
                               Order                                 : INTEGER;
                               VAR LArr                              : Matrix   );

PROCEDURE DERIV              ( X                                     : Matrix;
                               VAR XDot                              : Matrix   );

PROCEDURE PERTACCEL          ( R,V                                   : Vector;
                               ITime                                 : EXTENDED;
                               WhichOne,Order                        : INTEGER;
                               BC                                    : EXTENDED;
                               VAR APert                             : Vector   );

PROCEDURE PDeriv             ( ITime                                 : EXTENDED;
                               X                                     : Matrix;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR XDot                              : Matrix   );

PROCEDURE RK4                ( ITime                                 : EXTENDED;
                               DtDay                                 : EXTENDED;
                               VAR XDot                              : Matrix;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR X                                 : Matrix   );

PROCEDURE RKF45              ( ITime                                 : EXTENDED;
                               VAR DtDay                             : EXTENDED;
                               VAR XDot                              : Matrix;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR X                                 : Matrix   );

PROCEDURE COWELL             ( R,V                                   : Vector;
                               ITime,FTime,DtDay                     : EXTENDED;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR R1,V1                             : Vector   );

PROCEDURE ATMOS              ( R                                     : Vector;
                               VAR Rho                               : EXTENDED );

{ ------------------------- Constants used in this Library ---------------------
|
|     Pi         : EXTENDED =     3.14159265358979;
|     TwoPi      : EXTENDED =     6.28318530717959;
|     Rad2Deg    : EXTENDED =    57.29577951308230;
|     Deg2Rad    : EXTENDED =     0.01745329251994;
|     OmegaEarth : EXTENDED =    0.05883359980154919;
|     J2         : EXTENDED =    0.00108263;
|     J3         : EXTENDED =   -0.00000254;
|     J4         : EXTENDED =   -0.00000161;
|     GMS        : EXTENDED =    3.329529364E05;
|     GMM        : EXTENDED =    0.01229997;
|     AUER       : EXTENDED = 149597870.0/6378.1363;
|
 ----------------------------------------------------------------------------- }

(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PKEPLER
|
|  This PROCEDURE propagates a satellite's position and velocity vector over
|    a given time period accounting for perturbations caused by J2.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ro          - original position vector       ER
|    Vo          - original velocity vector       ER/TU
|    NDot        - Time rate of change of n       rad/TU
|    NDDot       - Time accel of change of n      rad/TU2
|    DtTU        - Change in time                 TU
|
|  Outputs       :
|    R           - updated position vector        ER
|    V           - updated velocity vector        ER/TU
|
|  Locals        :
|    P           - Semi-paramter                  ER
|    A           - semior axis                    ER
|    Ecc         - eccentricity
|    incl        - inclination                    rad
|    Argp        - argument of periapsis          rad
|    ArgpDot     - change in argument of perigee  rad/TU
|    Omega       - longitude of the asc node      rad
|    OmegaDot    - change in Omega                rad
|    E0          - eccentric anomaly              rad
|    E1          - eccentric anomaly              rad
|    M           - mean anomaly                   rad/TU
|    MDot        - change in mean anomaly         rad/TU
|    ArgLat      - argument of latitude           rad
|    ArgLatDot   - change in argument of latitude rad/TU
|    TrueLon     - true longitude of vehicle      rad
|    TrueLonDot  - change in the true longitude   rad/TU
|    LonPero     - longitude of periapsis         rad
|    LonPeroDot  - longitude of periapsis change  rad/TU
|    N           - mean angular motion            rad/TU
|    NUo         - true anomaly                   rad
|    J2oP2       - J2 over p sqyared
|    Sinv,Cosv   - Sine and Cosine of Nu
|
|  Coupling:
|    RV2COE       - Orbit Elements from position and Velocity vectors
|    COE2RV       - Position and Velocity Vectors from orbit elements
|    NEWTONM     - Newton Rhapson to find Nu and Eccentric anomaly
|    MODFUNC     - MOD operation for REAL variables
|
|  References    :
|    Vallado       2007, 687, Alg 64
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE PKEPLER            ( Ro,Vo                                 : Vector;
                               ndot,nddot,DtTU                       : EXTENDED;
                               VAR R,V                               : Vector    );
   CONST
     TwoPi : EXTENDED =  6.28318530717959;
     Pi    : EXTENDED =  3.14159265358979;
     J2    : EXTENDED =  {0.0216526;20x} 0.00108263;  {40x 0.0433052;}
     Small : EXTENDED =  0.000001;
   VAR
     Tndto3,p, a, Ecc, Incl, Omega, Argp, Nu, M, ArgLat, TrueLon, LonPero, OmegaDot, E0,
     ArgpDot, MDot, ArgLatDot, TrueLonDot, LonPerDot, n, J2oP2 : EXTENDED;
   BEGIN
     RV2COE( Ro,Vo,  p,a,Ecc,incl,Omega,Argp,Nu,M,ArgLat,TrueLon,LonPero);
     n:= SQRT(1.0/(A*A*A));

     { ------------- Find the value of J2 perturbations ------------- }
     J2oP2   := (n*1.5*J2) / (p*p);
{     NBar    := n*( 1.0 + J2oP2*SQRT(1.0-Ecc*Ecc)* (1.0 - 1.5*SIN(Incl)*SIN(Incl)) );}
     OmegaDot:= -J2oP2 * COS(Incl);
     ArgpDot :=  J2oP2 * (2.0-2.5*SIN(Incl)*SIN(Incl));
     MDot    :=  N;

     Tndto3:= 2.0*NDot*DtTU / (3.0*n);
     a     := a - Tndto3 * a;
{     edot  := -Tndto3 * (1.0-Ecc)/DtTU;}
     Ecc   := Ecc - Tndto3 * (1.0-Ecc);
     p     := a*(1.0 - Ecc*Ecc);

     { ----- Update the orbital elements for each orbit type -------- }
     IF Ecc < Small THEN
         { -------------  Circular Equatorial  ---------------- }
         IF ( incl < Small ) or ( ABS(Incl-Pi) < Small ) THEN
           BEGIN
             TrueLonDot:= OmegaDot + ArgpDot + MDot;
             TrueLon   := TrueLon  + TrueLonDot * DtTU;
             TrueLon   := MODFUNC(TrueLon, TwoPi);
           END
           ELSE
           { -------------  Circular Inclined    -------------- }
           BEGIN
              Omega    := Omega + OmegaDot * DtTU;
              Omega    := MODFUNC(Omega, TwoPi);
              ArgLatDot:= ArgpDot + MDot;
              ArgLat   := ArgLat + ArgLatDot * DtTU;
              ArgLat   := MODFUNC(ArgLat, TwoPi);
            END
        ELSE
          { -- Elliptical, Parabolic, Hyperbolic Equatorial --- }
          IF ( incl < Small ) or ( ABS(Incl-Pi) < Small ) THEN
            BEGIN
              LonPerDot:= OmegaDot + ArgpDot;
              LonPero  := LonPero + LonPerDot * DtTU;
              LonPero  := MODFUNC(LonPero, TwoPi);
              M        := M + MDOT*DtTU + NDot*DtTU*DtTU + NDdot*DtTU*DtTU*DtTU;
              M        := MODFUNC(M, TwoPi);
              NEWTONM( Ecc,M,  e0,Nu );
            END
            ELSE
            { --- Elliptical, Parabolic, Hyperbolic Inclined -- }
            BEGIN
              Omega:= Omega + OmegaDot * DtTU;
              Omega:= MODFUNC(Omega, TwoPi);
              Argp := Argp  + ArgpDot  * DtTU;
              Argp := MODFUNC(Argp, TwoPi);
              M    := M + MDOT*DtTU + NDot*DtTU*DtTU + NDdot*DtTU*DtTU*DtTU;
              M    := MODFUNC(M, TwoPi);
              NEWTONM( Ecc,M,  e0,Nu );
            END;

        { ------------- Use COE2RV to find new vectors --------------- }
        COE2RV( P,Ecc,Incl,Omega,Argp,Nu,ArgLat,TrueLon,LonPero,  R,V );
   END; { PROCEDURE PKEPLER }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE J2DRAGPERT
|
|  This PROCEDURE calculates the perturbations for the PREDICT problem
|    involving secular rates of change resulting from J2 and Drag only.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    incl        - Inclination                    rad
|    Ecc         - Eccentricity
|    N           - Mean Motion                    rad/TU
|    NDot        - Mean Motion rate               rad / 2TU2
|
|  Outputs       :
|    OmegaDot    - Long of Asc Node rate          rad / TU
|    ArgpDot     - Argument of perigee rate       rad / TU
|    EDot        - Eccentricity rate              / TU
|
|  Locals        :
|    P           - Semiparameter                  ER
|    A           - Semimajor axis                 ER
|    NBar        - Mean Mean motion               rad / TU
|
|  Coupling      :
|    POWER         Raise a base to a POWER
|
|  References    :
|    Vallado       2007, 645, Eq 9-37, 647, Eq 9-39, 648, Eq 9-50
|
 ----------------------------------------------------------------------------- }

PROCEDURE J2DragPert         ( Incl,Ecc,N,NDot                       : EXTENDED;
                               VAR OmegaDOT,ArgpDOT,EDOT             : EXTENDED );
   CONST
     J2  : EXTENDED =  0.00108263;
   VAR
     P,A,NBar : EXTENDED;
   BEGIN
     a   := POWER( 1.0/n , 2.0/3.0 );
     p   := a*(1.0-Ecc*Ecc);
     NBar:= n*( 1.0+1.5*J2*(SQRT(1.0-Ecc*Ecc) /
            (p*p))*(1.0-1.5*SIN(incl)*SIN(incl) ));

     { ------------------------ Find DOT terms  --------------------- }
     OmegaDot:= -1.5*( J2/(p*p) ) * COS(incl) * NBar;
     ArgpDot :=  1.5*( J2/(p*p) ) * (2.0-2.5*SIN(incl)*SIN(incl)) * Nbar;
     EDot    := -(4.0/3.0) * (1.0-Ecc) * (NDot/Nbar);

   END; { PROCEDURE J2DragPert  }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PREDICT
|
|  This PROCEDURE determines the azimuth and elevation for the viewing
|    of a satellite from a known ground SITE.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian Date of desired obs     Day
|    Latgd       - Geodetic Latitude of SITE      -Pi to Pi rad
|    LST         - Local SIDEREAL Time            -2Pi to 2Pi rad
|    R           - updated position vector        ER
|    V           - updated velocity vector        ER/TU
|    RS          - IJK SITE Vector                ER
|    WhichKind   - Type of Sunrise                'S''C''N''A'
|
|  OutPuts       :
|    Rho         - Range from SITE to satellite   ER
|    Az          - Azimuth                        rad
|    El          - Elevation                      rad
|    TRtAsc      - Topo Right ascension           rad
|    TDecl       - Topo Declination               rad
|    Vis         - Visibility
|                  'Radar SUN'   - both in SUN
|                  'Eye'  - SITE dark, sat in SUN
|                  'Night     '  - both dark
|                  'not visible' - sat below horz
|  Locals        :
|    Temp        - Temporary Real value
|    SRtAsc      - Suns Right ascension           rad
|    SDecl       - Suns Declination               rad
|    SatAngle    - ANGLE between IJK SUN and Sat  rad
|    Dist        - Ppdculr dist of sat from RSun  ER
|    rr          - Range rate
|    Drr         - Range acceleration
|    Dtrtasc     - Topocentric rtasc rate
|    DRho        - Slant range rate
|    DAz         - Azimuth rate
|    Del         - Elevation rate
|    SunAngle    - ANGLE between SUN and SITE     rad
|    AngleLimit  - ANGLE for twilight conditions  rad
|    RhoVec      - SITE to sat vector in SEZ      ER
|    TempVec     - Temporary vector
|    RHoV        - SITE to sat vector in IJK      ER
|    RSun        - SUN vector                     AU
|    C           - Temporary Vector
|
|  Coupling      :
|    SUN         - Position vector of SUN
|    CROSS       - CROSS Product of two vectors
|    ROT2,ROT3   - Rotations about 2nd and 3rd axis
|    LNCOM1      - Combination of a vector and a scalar
|    LNCOM2      - Combination of two vectors and two scalars
|    RV_RAZEL    - Conversion with vectors and range azimuth elevation
|    RV_TRADEC   - Conversion with topocentric right ascension declination
|    ANGLE       - ANGLE between two vectors
|
|  References    :
|    Vallado       2007, 900, Alg 73, Ex 11-6
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE PREDICT            ( JD,latgd,LST                          : EXTENDED;
                               r,v,rs                                : Vector;
                               WhichKind                             : CHAR;
                               VAR Rho,Az,El,tRtasc,tDecl            : EXTENDED;
                               VAR Vis                               : Str11   );
   CONST
     Small      : EXTENDED =     0.000001;
     Deg2Rad    : EXTENDED =     0.01745329251994;
     HalfPi     : EXTENDED =     1.57079632679490;
     AUER       : EXTENDED = 149597870.0/6378.1363;
   VAR
     RhoVec, TempVec, RhoV, RSun, C   : Vector;
     rr, drr, dtrtasc, dtdecl, Temp, SRtAsc, SDecl, Dist,
     drho, daz, del, SunAngle, SatAngle, AngleLimit : EXTENDED;
   BEGIN
     { ----------------------- Initialize values -------------------- }
     Az    :=  0.0;
     El    :=  0.0;
     Rho   :=  0.0;
     TRtAsc:=  0.0;
     TDecl :=  0.0;

     { ------- Find IJK range vector from SITE to satellite --------- }
     LNCOM2( 1.0,-1.0,R,RS,  RhoV );
     Rho:= RhoV[4];

     { -------- Calculate Topocentric Rt Asc and Declination -------- }
     RV_TRADEC( r,v,rs,TOO,  rr,trtasc,tdecl,Drr,Dtrtasc,Dtdecl );

     { ----------------------- Rotate to SEZ ------------------------ }
     ROT3( RhoV,       LST   ,  TempVec );
     ROT2( TempVec,HalfPi-Latgd,   RhoVec );

     { ---------------- Check visibility constraints ---------------- }
     { ------------------- Is it above the Horizon ------------------ }
     IF RhoVec[3] > 0.0 THEN
       BEGIN
         { ---------- Is the SITE in the LIGHT, or the dark? -------- }
         SUN( JD,RSun,SRtAsc,SDecl );
         LNCOM1( AUER,RSun, RSun );
         ANGLE( RSun,RS, SunAngle );
         CASE WHICHKind OF
           'S' : AngleLimit:= (90.0 + 50.0/60.0)*Deg2Rad;
           'C' : AngleLimit:=  96.0*Deg2Rad;
           'N' : AngleLimit:= 102.0*Deg2Rad;
           'A' : AngleLimit:= 108.0*Deg2Rad;
         END;  { Case }

         IF SunAngle < AngleLimit THEN
             Vis:= 'Day        '
           ELSE
           BEGIN
             { ----------- This assumes a conical shadow ------------ }
             { ------ Is the satellite in the shadow or not? -------- }
             CROSS( RSun, R, C );
             SatAngle:= ASIN( C[4]/ (RSun[4]*R[4]) );
             Dist:= R[4]*COS( SatAngle - HalfPi );
             IF Dist > 1.0 THEN
                 Vis:= 'Terminator '
               ELSE
                 Vis:= 'Night      ';
           END;
       END
       ELSE
         Vis:= 'not visible';

     { ------------  Calculate Azimuth and Elevation  --------------- }
     RV_RAZEL( r,v,rs,latgd,LST,TOO,  rho,az,el,drho,daz,del );
   END;  { PROCEDURE PREDICT }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE DERIV
|
|  This PROCEDURE calculates the derivative of the two-body state vector for
|    use with the Runge-Kutta algorithm.  Note time is not needed.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    X           - State Vector                   ER, ER/TU
|
|  Outputs       :
|    XDot        - Derivative of State Vector     ER/TU,  ER/TU2
|
|  Locals        :
|    RCubed      - Cube of R
|
|  Coupling      :
|    ASSIGNVAL   - Assign a value to a matrix
|    GETVAL      - Get a value from a matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE DERIV              ( X                                     : Matrix;
                               VAR XDot                              : Matrix );
   VAR
     R,RCubed : EXTENDED;
   BEGIN
     R:= SQRT( SQR(GETVAL(X,1,1)) + SQR(GETVAL(X,2,1)) + SQR(GETVAL(X,3,1)) );
     RCubed:= R*R*R;

     { --------------------  Velocity Terms  ------------------------ }
     ASSIGNVAL( XDot,1,1,GETVAL(X,4,1) );
     ASSIGNVAL( XDot,2,1,GETVAL(X,5,1) );
     ASSIGNVAL( XDot,3,1,GETVAL(X,6,1) );

     { ------------------  Acceleration Terms  ---------------------- }
     ASSIGNVAL( XDot,4,1,-GETVAL(X,1,1) / RCubed );
     ASSIGNVAL( XDot,5,1,-GETVAL(X,2,1) / RCubed );
     ASSIGNVAL( XDot,6,1,-GETVAL(X,3,1) / RCubed );

   END;  { PROCEDURE DERIV }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE InitGravityField
|
|  This PROCEDURE reads and stores the gravity field for use in the program.
|    coefficients. The routine can be configured for either normalized or
|    unnormalized values.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Order       - Size of gravity field          1..70
|
|  Outputs       :
|    C           - Gravitational Coefficients
|    S           - Gravitational Coefficients
|
|  Locals        :
|    RCubed      - Cube of R
|
|  Coupling      :
|    INITMATRIX  - Initialize a matrix and fil with 0.0's
|    ASSIGNVAL   - Assign a value to a matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE InitGravityField   ( Order                                 : INTEGER;
                               VAR C,S                               : Matrix );
   VAR
     GravFile : TEXT;
     l, m, cexp, sexp : INTEGER;
     cnor, snor, Cval, Sval : EXTENDED;
   BEGIN
     C:= NIL;
     S:= NIL;
     INITMATRIX(70,70,1,1, C );
     INITMATRIX(70,70,1,1, S );

     ASSIGN( GravFile,'c:\tplib\JGM2.out' );
     RESET( GravFile );

     WHILE NOT EOF(GravFile) DO
       BEGIN
         Readln( GravFile, l,m,Cnor,cexp,Snor,sexp,cval,sval );
         ASSIGNVAL( C,l,m, Cval ); { unnormalized values }
         ASSIGNVAL( S,l,m, Sval );
       END;

     CLOSE( GravFile );

   END;  { PROCEDURE InitGravityField }

{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LegPoly
|
|  This PROCEDURE finds the Legendre polynomials for the gravity field.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Latgc       - Geocentric Latitude of SITE    -Pi to Pi rad
|    Order       - Size of gravity field          1..70
|
|  Outputs       :
|    LArr        - Array of Legendre Polynomials
|
|  Locals        :
|    L,m         - Indices of gravitational potential
|
|  Coupling      :
|    INITMATRIX  - Initialize a matrix and fil with 0.0's
|    ASSIGNVAL   - Assign a value to a matrix
|    GETVAL      - Get a value from a matrix
|
|
|  References    :
|    Vallado       2007, 593, Eq 8-56
|
 ----------------------------------------------------------------------------- }

PROCEDURE LegPoly            ( Latgc                                 : EXTENDED;
                               Order                                 : INTEGER;
                               VAR LArr                              : Matrix );
   VAR
     L,m : INTEGER;
   BEGIN
     LArr:= NIL;
     INITMATRIX( Order,Order,0,0, LArr );

     { ----------------------- Initial values ----------------------- }
     ASSIGNVAL( LArr,0,1, 0.0 );
     ASSIGNVAL( LArr,0,0, 1.0 );
     ASSIGNVAL( LArr,1,0, SIN(Latgc) );
     ASSIGNVAL( LArr,1,1, COS(Latgc) );

     { -------------------- Perform Recursions ---------------------- }
     FOR L:= 2 to Order DO
       BEGIN
         ASSIGNVAL( LArr,0,L-1, 0.0 );
         FOR m:= 0 to L DO
           BEGIN
             IF m = 0 THEN
                 ASSIGNVAL( LArr,L,0, ( (2*L-1)*GETVAL( LArr,1,0 )*GETVAL( LArr,L-1,0)
                               - (L-1)*GETVAL( LArr,L-2,0) )/L )
               ELSE
                 IF m = L THEN
                     ASSIGNVAL( LArr,L,m, (2*L-1)*GETVAL( LArr,1,1)*GETVAL( LArr,L-1,m-1) )
                   ELSE
                     ASSIGNVAL( LArr,L,m, GETVAL( LArr,L-2,m)
                                 + (2*L-1)*GETVAL( LArr,1,1)*GETVAL( LArr,L-1,m-1) );
             IF Show = 'Y' THEN
                 writeln( L:3,m:3,GETVAL( LArr,L,m):11:7 );
           END;  { For m }
       END;  { For L }

   END;  { PROCEDURE LegPoly }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE FullGeop
|
|  This PROCEDURE finds the Legendre polynomial value for the gravity field
|    for a given order.
|
|  Algorithm     : Find the answer
|                  Step up recursion
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Order       - Size of gravity field          1..70
|
|  Outputs       :
|    C           - Gravitational Coefficients
|    S           - Gravitational Coefficients
|
|  Locals        :
|    RCubed      - Cube of R
|
|  Coupling      :
|    IJK2LLA- Find sub satellite point
|
|  References    :
|    Vallado       2007,
|
 ----------------------------------------------------------------------------- }

PROCEDURE FullGeop           ( R,V                                   : Vector;
                               ITime                                 : EXTENDED;
                               WhichOne, Order                       : INTEGER;
                               BC                                    : EXTENDED;
                               C,S                                   : Matrix;
                               VAR APert                             : Vector   );
   VAR
     L, m : INTEGER;
     LArr : Matrix;
     OORDelta, Temp, OOr, SumM1, SumM2, SumM3, DistPartr,
     DistPartPhi, DistPartLon, RDelta, Latgc, latgd, hellp, Lon : EXTENDED;
   BEGIN
     { -------------------- find latgc and lon ---------------------- }
     IJK2LLA( R,ITime, latgc,latgd,lon,hellp );

     { --------------------- Find Legendre polynomials -------------- }
     LegPoly( Latgc,Order, LArr );

     { ---------- Partial derivatives of disturbing potential ------- }
     OOr        := 1.0/r[4];
     DistPartR  := 0.0;
     DistPartPhi:= 0.0;
     DistPartLon:= 0.0;
     Temp       := OOr;

     FOR L:= 2 to Order DO
       BEGIN
         Temp:= Temp * OOr;  { will do the power as each L is indexed }
         SumM1:= 0.0;
         SumM2:= 0.0;
         SumM3:= 0.0;

         FOR m:= 0 to L DO
           BEGIN
             SumM1:= SumM1 + GETVAL(LArr,L,m) *
                           ( GETVAL(C,L,m)*COS(m*Lon) + GETVAL(S,L,m)*SIN(m*Lon) );
             SumM2:= SumM2 + (GETVAL(C,L,m)*COS(m*Lon) + GETVAL(S,L,m)*SIN(m*Lon)) *
                             ( GETVAL(LArr,L,m+1) - m*GETVAL(LArr,L,m)*TAN(Latgc) );
             SumM3:= SumM3 + m*GETVAL(LArr,L,m) *
                           ( GETVAL(S,L,m)*COS(m*Lon) - GETVAL(C,L,m)*SIN(m*Lon) );
           END;  { For m }

         DistPartR  := DistPartR   + Temp*(L+1)*SumM1;
         DistPartPhi:= DistPartPhi + Temp*SumM2;
         DistPartLon:= DistPartLon + Temp*SumM3;
       END;  { For L }

     DistPartR  := -OOr*OOr * SumM1;
     DistPartPhi:=  OOr     * SumM2;
     DistPartLon:=  OOr     * SumM3;

     { ---------- Non-spherical pertubative acceleration ------------ }
     RDelta  := SQRT( r[1]*r[1] + r[2]*r[2] );
     OORdelta:= 1.0/RDelta;
     Temp    := OOr*DistPartR - r[3]*OOr*OOr*OORDelta*DistPartPhi;

     APert[1]:= Temp*r[1] - OORDelta*DistPartLon*r[2];
     APert[2]:= Temp*r[2] + OORDelta*DistPartLon*r[1];
     APert[3]:= OOr*DistPartR*r[3] + OOR*OOR*RDelta*DistPartPhi;
{MAG(Apert);}
   END;  { PROCEDURE FullGeop }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PERTACCEL
|
|  This PROCEDURE calculates the actual value of the perturbing acceleration.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - Radius vector                  ER
|    V           - Velocity vector                ER/TU
|    Time        - Initial time (Julian Date)     Days from 4713 BC
|    WhichOne    - Which perturbation to calc     1 2 3 4 5 ...
|    BC          - Ballistic Coefficient          kg/m2
|
|  Outputs       :
|    APert       - Perturbing acceleration        ER/TU2
|
|  Locals        :
|    rs2         - SUN radius vector **2
|    rs3         - SUN radius vector **3
|    rm2         - MOON radius vector **2
|    rm3         - MOON radius vector **3
|    r32         - "z" component of Radius vec **2
|    r33         - "z" component of Radius vec **3
|    r34         - "z" component of Radius vec **4
|    r2          - Radius vector **2
|    r3          - Radius vector **3
|    r4          - Radius vector **4
|    r5          - Radius vector **5
|    r7          - Radius vector **7
|    Beta        -
|    Temp        - Temporary Real Value
|    rho         - Atmospheric Density
|    Va          - Relative Velocity Vector       ER / TU
|    RSun        - Radius Vector to SUN           AU
|    RMoon       - Radius Vector to MOON          ER
|    RtAsc       - Right Ascension                rad
|    Decl        - Declination                    rad
|    AUER        - Conversion from AU to ER
|    Temp1       -
|    Temp2       -
|    i           - Index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    SUN         - SUN vector
|    MOON        - MOON vector
|    ATMOS       - Atmospheric density
|
 ----------------------------------------------------------------------------- }

PROCEDURE PERTACCEL          ( R,V                                   : Vector;
                               ITime                                 : EXTENDED;
                               WhichOne, Order                       : INTEGER;
                               BC                                    : EXTENDED;
                               VAR APert                             : Vector    );
   CONST
     OmegaEarth : EXTENDED =    0.05883359980154919;
     J2         : EXTENDED =    0.00108263;
     J3         : EXTENDED =   -0.00000254;
     J4         : EXTENDED =   -0.00000161;
     GMS        : EXTENDED =    3.329529364E05;
     GMM        : EXTENDED =    0.01229997;
   VAR
     i : INTEGER;
     Va, RSun, RMoon : VECTOR;
     rs2, rm2, rs3, rm3, r32, r33, r34, r2, r3, r4, r5, r7, AuER,
     Beta, Temp, rho, srtasc, sdecl, mrtasc, mdecl, Temp1, Temp2 : EXTENDED;
   BEGIN
     MAG( R );   { extra call? }
     MAG( V );
     R2 := r[4]*r[4];
     R3 := R2*r[4];
     R4 := R2*R2;
     R5 := R2*R3;
     R7 := R5*R2;
     R32:= r[3]*r[3];
     R33:= R32*r[3];
     R34:= R32*R32;

     CASE WhichOne OF
      { ------------------   J2 Acceleration   ---------------------- }
       1 : BEGIN
             Temp1:=  (-1.5*J2) / R5;
             Temp2:=  1.0 - (5.0*R32) / R2;
             APert[1]:= Temp1*r[1] * Temp2; { recheck with formulae }
             APert[2]:= Temp1*r[2] * Temp2;
             APert[3]:= Temp1*r[3] * ( 3.0-(5.0*R32) / R2 );
             MAG( APert );
           END;

      { -------------------   J3 Acceleration   --------------------- }
       2 : BEGIN
             Temp1:=  (-2.5*J3) / R7;
             Temp2:=  3.0*r[3]-(7.0*R33) / R2;
             APert[1]:= Temp1*r[1] * Temp2;
             APert[2]:= Temp1*r[2] * Temp2;
             IF ABS( r[3] ) > 0.0000001 THEN
                 APert[3]:= Temp1*r[3] * ((6.0*r[3])-((7.0*R33)
                              / R2) - ( (3.0*r2) / (5.0*r[3]) ))
               ELSE
                 APert[3]:= 0.0;
             MAG( APert );
           END;

      { ------------------    J4 Acceleration   --------------------- }
       3 : BEGIN
             Temp1:=  (-1.875*J4) / R7;
             Temp2:=  1.0-((14.0*R32)/R2)+((21.0*R34) / R4);
             APert[1]:= Temp1*r[1] * Temp2;
             APert[2]:= Temp1*r[2] * Temp2;
             APert[3]:= Temp1*r[3] * (5.0-((70.0*R32)/(3.0*R2))+((21.0*R34) / R4 ));
             MAG( APert );
           END;

      { ------------------   SUN Acceleration   --------------------- }
       4 : BEGIN
             SUN( ITime,RSun,SRtAsc,SDecl );
             AuER:= 149597870.0/6378.137;
             FOR i:= 1 to 4 DO
                 RSun[i]:= RSun[i]*AuER;   { chg AU's to ER's }

             RS2:= RSun[4]*RSun[4];
             RS3:= RS2*RSun[4];
             Temp:= DOT( R,RSun );
             Temp1:= -GMS/RS3;
             Temp2:= 3.0*Temp/RS2;
             APert[1]:= Temp1 * (r[1] - Temp2*RSun[1]);
             APert[2]:= Temp1 * (r[2] - Temp2*RSun[2]);
             APert[3]:= Temp1 * (r[3] - Temp2*RSun[3]);
             MAG( APert );
           END;

      { ------------------  MOON Acceleration   --------------------- }
       5 : BEGIN
             MOON( ITime,RMoon,MRtAsc,MDecl );
             RM2:= RMoon[4]*RMoon[4];
             RM3:= RM2*RMoon[4];
             Temp:= DOT( R,RMoon );
             Temp1:= -GMM/RM3;
             Temp2:= 3.0*Temp/RM2;
             APert[1]:= Temp1 * (r[1] - Temp2*RMoon[1]);
             APert[2]:= Temp1 * (r[2] - Temp2*RMoon[2]);
             APert[3]:= Temp1 * (r[3] - Temp2*RMoon[3]);
             MAG( APert );
           END;

      { ------------------  Drag Acceleration   --------------------- }
       6 : BEGIN
             Va[1]:= V[1] + (OmegaEarth*r[2]);  { ER/TU }
             Va[2]:= V[2] - (OmegaEarth*r[1]);
             Va[3]:= V[3];
             MAG( Va );

             ATMOS( R, Rho );

             Temp:= -1000.0 * Va[4] * 0.5*Rho* ( 1.0/BC );
             APert[1]:= Temp*Va[1];
             APert[2]:= Temp*Va[2];
             APert[3]:= Temp*Va[3];
             MAG( APert );
           END;

      { ------------------ Solar Acceleration   --------------------- }
       7 : BEGIN
             SUN( ITime,RSun,SRtAsc,SDecl );

             Beta:= 0.4;                         { reflectivity     }
             APert[4]:= (Beta*2.0*4.51E-06)/BC;  { assume Csr = 2.0 }
             Temp:= -APert[4]/RSun[4];
             APert[1]:= Temp*RSun[1];
             APert[2]:= Temp*RSun[2];
             APert[3]:= Temp*RSun[3];
           END;


      { -------------------- Square Gravity Field ------------------- }
       10 : BEGIN
              FullGeop( R,V,ITime,WhichOne,Order,BC,C,S,APert );
            END;
     END;  { Case }

   END;  { PROCEDURE PERTACCEL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PDERIV
|
|  This PROCEDURE calculates the derivative of the state vector for use with
|    the Runge-Kutta algorithm.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    ITime       - Initial Time (Julian Date)     Days from 4713 BC
|    X           - State Vector                   ER  ,  ER/TU
|    DerivType   - String of which perts to incl  'Y' and 'N'
|                  Options are in order : J2, J3,
|                  J4, SUN, MOON, Drag, Solarrad
|    BC          - Ballistic Coefficient          kg/m2
|
|  Outputs       :
|    XDot        - Derivative of State Vector     ER/TU, ER/TU2
|
|  Locals        :
|    RCubed      - Radius vector cubed            ER3
|    Ro          - Radius vector                  ER
|    Vo          - Velocity vector                ER/TU
|    APert       - Perturbing acceleration        ER/TU2
|    TempPert    - Temporary acceleration         ER/TU2
|    i           - Index
|
|  Coupling      :
|    PERTACCEL   - Calculates the actual values of each perturbing acceleration
|    ADDVEC      - Adds two vectors together
|    ASSIGNVAL   - Assign a value to a matrix
|    GETVAL      - Get a value from a matrix
|    MAG         - Magnitude of a vector
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE PDeriv             ( ITime                                 : EXTENDED;
                               X                                     : Matrix;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR XDot                              : Matrix );
   VAR
     i  : INTEGER;
     RCubed : EXTENDED;
     Ro, Vo, APert, TempPert : Vector;
   BEGIN
     FOR i:= 1 to 3 DO
       BEGIN
         APert[i]:= 0.0;
         Ro[i] := GETVAL(x,i,1);
         Vo[i] := GETVAL(x,i+3,1);
       END;
     MAG( Ro );
     MAG( Vo );
     APert[4]:= 0.0;

     RCubed:= Ro[4]*Ro[4]*Ro[4];

     { ---------------------  Velocity Terms  ----------------------- }
     ASSIGNVAL( XDot,1,1, GETVAL(X,4,1) );
     ASSIGNVAL( XDot,2,1, GETVAL(X,5,1) );
     ASSIGNVAL( XDot,3,1, GETVAL(X,6,1) );

     { -------------------  Acceleration Terms  --------------------- }
     IF DerivType[1] = 'Y' THEN
         PERTACCEL( Ro,Vo,ITime,1,Order,BC, APert );
     IF DerivType[2] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,2,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;
     IF DerivType[3] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,3,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;
     IF DerivType[4] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,4,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;
     IF DerivType[5] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,5,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;
     IF DerivType[6] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,6,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;
     IF DerivType[7] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,7,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;

     { --------------------- new full gravity field ----------------- }
     IF DerivType[10] = 'Y' THEN
       BEGIN
         PERTACCEL( Ro,Vo,ITime,10,Order,BC, TempPert );
         ADDVEC( TempPert,APert,APert );
       END;

     ASSIGNVAL( XDot,4,1,(-GETVAL(X,1,1) / RCubed) + APert[1] );
     ASSIGNVAL( XDot,5,1,(-GETVAL(X,2,1) / RCubed) + APert[2] );
     ASSIGNVAL( XDot,6,1,(-GETVAL(X,3,1) / RCubed) + APert[3] );

   END;  { PROCEDURE PDeriv }
{}
{ ------------------------------------------------------------------------------
|
|                                PROCEDURE RK4
|
|  This PROCEDURE is a fourth order Runge-Kutta integrator for a 6 dimension
|    First Order differential equation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|                                 12 Nov 1993 - fix for TU, DT, etc.
|  Inputs          Description                    Range / Units
|    ITime       - Initial Time (Julian Date)     Days from 4713 BC
|    DtDay       - Step size                      Day
|    XDot        - Derivative of State Vector
|    DerivType   - String of which perts to incl  'Y' and 'N'
|                  Options are in order : J2, J3,
|                  J4, SUN, MOON, Drag, Solarrad
|    BC          - Ballistic Coefficient          kg/m2
|    X           - State vector at initial time   ER, ER/TU
|
|  Outputs       :
|    X           - State vector at new time       ER, ER/TU
|
|  Locals        :
|    K           - Storage for values of state
|                   vector at different times
|    Temp        - Storage for state vector
|    TempTime    - Temporary time storage half
|                   way between DtDay             Day
|    J           - Index
|    DtTU        - Step size                      TU
|
|  Coupling      :
|    DERIV       - PROCEDURE for Derivatives of EOM
|    PDeriv      - PROCEDURE for Perturbed Derivatives of EOM
|    INITMATRIX  - Initialize a matrix and fil with 0.0's
|    ASSIGNVAL   - Assign a value to a matrix
|    GETVAL      - Get a value from a matrix
|    DELMATRIX   - Delete a matrix
|
|  References    :
|    Vallado       2007, 526
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE RK4                ( ITime                                 : EXTENDED;
                               DtDay                                 : EXTENDED;
                               VAR XDot                              : Matrix;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR X                                 : Matrix );
   CONST
     TUDay      : EXTENDED =     0.0093380913806;
   VAR
     j : INTEGER;
     K, Temp  : Matrix;
     DtTU, TempTime    : EXTENDED;
   BEGIN
     { ---------------------- Initialize X DOT ---------------------- }
     K   := NIL;
     Temp:= NIL;  { local }
     INITMATRIX( 6,3,1,1, K );
     INITMATRIX( 6,1,1,1, Temp );

     DtTU:= DtDay/TUDay;

     { -------------- Evaluate 1st Taylor Series Term --------------- }
     IF DerivType[10] = '2' THEN
         DERIV( X, XDot )
       ELSE
         PDeriv( ITime,X,DerivType,Order,BC, XDot );

     TempTime:= ITime + DtDay*0.5;

     { -------------- Evaluate 2nd Taylor Series Term --------------- }
     FOR j:= 1 to 6 DO
       BEGIN
         ASSIGNVAL( K,   J,1,  DtTU*GETVAL( XDot,J,1) );
         ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + 0.5*GETVAL( K,J,1 ) );
       END;
     IF DerivType[10] = '2' THEN
         DERIV( Temp, XDot )
       ELSE
         PDeriv( TempTime,Temp,DerivType,Order,BC,  XDot );

     { -------------- Evaluate 3rd Taylor Series Term --------------- }
     FOR j:= 1 to 6 DO
       BEGIN
         ASSIGNVAL( K,   J,2,  DtTU*GETVAL( XDot,J,1) );
         ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + 0.5*GETVAL( K,J,2 ) );
       END;
     IF DerivType[10] = '2' THEN
         DERIV( Temp, XDot )
       ELSE
         PDeriv( TempTime,Temp,DerivType,Order,BC,  XDot );

     { -------------- Evaluate 4th Taylor Series Term --------------- }
     FOR j:= 1 to 6 DO
       BEGIN
         ASSIGNVAL( K,   J,3,  DtTU*GETVAL( XDot,J,1) );
         ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + GETVAL( K,J,3 ) );
       END;
     IF DerivType[10] = '2' THEN
         DERIV( Temp, XDot )
       ELSE
         PDeriv( ITime+DtDay,Temp,DerivType,Order,BC,  XDot );

     { ------- Update the State vector, perform integration --------- }
     FOR j:= 1 to 6 DO
         ASSIGNVAL( X,J,1,  GETVAL( X,J,1 ) +
               ( GETVAL( K,J,1 ) + 2.0*( GETVAL( K,J,2 )+ GETVAL( K,J,3 ) )
               + DtTU*GETVAL( XDot,J,1 ) ) / 6.0 );

     DELMATRIX( K );
     DELMATRIX( Temp );
   END;  { PROCEDURE RK4 }
{}
{ ------------------------------------------------------------------------------
|
|                                PROCEDURE RKF45
|
|  This PROCEDURE is a fourth order Runge-Kutta-Fehlberg integrator for a 6-D
|    First Order differential equation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    ITime       - Initial Time (Julian Date)     Days from 4713 BC
|    DtDay       - Step size                      Day
|    XDot        - Derivative of State Vector
|    DerivType   - String of which perts to incl  'Y' and 'N'
|                  Options are in order : J2, J3,
|                  J4, SUN, MOON, Drag, Solarrad
|    BC          - Ballistic Coefficient          kg/m2
|    X           - State vector at initial time   ER, ER/TU
|
|  Outputs       :
|    X           - State vector at new time       ER, ER/TU
|
|  Locals        :
|    K           - Storage for values of state
|                    vector at different times
|    Temp        - Storage for state vector
|    TempTime    - Temporary time storage half
|                    way between DtDay            Day
|    J           - Index
|    DtTU        - Step size                      TU
|
|  Coupling      :
|    DERIV       - PROCEDURE for Derivatives of EOM
|    PDeriv      - PROCEDURE for Perturbed Derivatives of EOM
|    INITMATRIX  - Initialize a matrix and fil with 0.0's
|    ASSIGNVAL   - Assign a value to a matrix
|    GETVAL      - Get a value from a matrix
|    DELMATRIX   - Delete a matrix
|
|  References    :
|    Vallado       2007, 526
|
 ----------------------------------------------------------------------------- }

PROCEDURE RKF45              ( ITime                                 : EXTENDED;
                               VAR DtDay                             : EXTENDED;
                               VAR XDot                              : Matrix;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR X                                 : Matrix );
   CONST
     Small      : EXTENDED =     0.000001; { this is pretty sensitive }
     TUDay      : EXTENDED =     0.0093380913806;
   VAR
     Ktr, J   : INTEGER;
     K, Temp  : Matrix;
     DtTU, HMin, HMax, TStop, Time, Err, S, TempTime, TempErr : EXTENDED;
{       CA : ARRAY[1..19] of EXTENDED;  c,cst:ARRAY[1..5] of EXTENDED;}
   BEGIN
     { ----------------------- Initialize X DOT --------------------- }
     k:= NIL;  Temp:= NIL;   { local }
     INITMATRIX( 6,6,1,1, K );
     INITMATRIX( 6,1,1,1, Temp );
     HMin := DtDay/64.0;
     HMax := DtDay*64.0;
     Time := ITime;
     TStop:= ITime + DtDay;
     DtTU := DtDay/TUDay;
(*
     CA[1]:= 0.2;        CA[5]:=  0.2;  CA[2]:= 0.3;   CA[6]:=  0.075;
     CA[10]:=  0.0225;   CA[3]:= 0.6;   CA[7]:=  0.3;  CA[11]:= -0.9;
     CA[8]:= -11.0/54.0; CA[12]:=  2.5; CA[4]:= 0.875; CA[9]:=  1631.0/55296.0;
     CA[13]:= 175.0/512.0;
     CA[14]:= 1.2;         CA[15]:= -70.0/27.0;       CA[16]:= 575.0/13824.0;
     CA[17]:= 35.0/27.0;   CA[18]:= 44275.0/110592.0; CA[19]:= 253.0/4096.0;
     C[1]:= 37.0/379.0;    Cst[1]:= 2825.0/27648.0;  C[2]:= 250.0/621.0;   Cst[2]:= 18575.0/48384.0;
     C[3]:= 125.0/594.0;   Cst[3]:= 13525.0/55296.0; C[4]:= 0.0;           Cst[4]:= 277.0/14336.0;
     C[5]:= 512.0/1771.0;  Cst[5]:= 0.25;
*)
     Ktr:= 1;
     WHILE Time < TStop DO
       BEGIN
         IF Time + DtDay > TStop THEN  { Make sure you END exactly on the step }
             DtDay:= TStop - Time;

         { -------------- Evaluate 1st Taylor Series Term ----------- }
         IF DerivType[10] = '2' THEN
             DERIV( X, XDot )
           ELSE
             PDeriv( Time,X,DerivType,Order,BC, XDot );

         TempTime:= Time + DtDay*0.25;
         { -------------- Evaluate 2nd Taylor Series Term ----------- }
         FOR j:= 1 to 6 DO
           BEGIN
             ASSIGNVAL( K,   J,1,  DtTU*GETVAL( XDot,J,1) );  { set # 1 }
             ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + 0.25*GETVAL( K,J,1 ) ); {get ready for 2 }
           END;
         IF DerivType[10] = '2' THEN
             DERIV( Temp, XDot )
           ELSE
             PDeriv( TempTime,Temp,DerivType,Order,BC,  XDot );

         TempTime:= Time + DtDay*0.375;
         { -------------- Evaluate 3rd Taylor Series Term ----------- }
         FOR j:= 1 to 6 DO
           BEGIN
             ASSIGNVAL( K,   J,2,  DtTU*GETVAL( XDot,J,1) );
             ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + 0.09375*GETVAL( K,J,1 )
                                                   + 0.28125*GETVAL( K,J,2 ) );
           END;
         IF DerivType[10] = '2' THEN
             DERIV( Temp, XDot )
           ELSE
             PDeriv( TempTime,Temp,DerivType,Order,BC,  XDot );

         TempTime:= Time + DtDay*12.0/13.0;

         { -------------- Evaluate 4th Taylor Series Term ----------- }
         FOR j:= 1 to 6 DO
           BEGIN
             ASSIGNVAL( K,   J,3,  DtTU*GETVAL( XDot,J,1) );
             ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + GETVAL( K,J,1 )*1932.0/2197.0
                   - GETVAL( K,J,2 )*7200.0/2197.0 + GETVAL( K,J,3 )*7296.0/2197.0 );
           END;
         IF DerivType[10] = '2' THEN
             DERIV( Temp, XDot )
           ELSE
             PDeriv( TempTime,Temp,DerivType,Order,BC,  XDot );

         { -------------- Evaluate 5th Taylor Series Term ----------- }
         FOR j:= 1 to 6 DO
           BEGIN
             ASSIGNVAL( K,   J,4,  DtTU*GETVAL( XDot,J,1) );
             ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) + GETVAL( K,J,1 )* 439.0/ 216.0
                   - GETVAL( K,J,2 ) * 8.0 + GETVAL( K,J,3 )*3680.0/ 513.0
                   - GETVAL( K,J,4 ) * 845.0/4104.0 );
           END;
         IF DerivType[10] = '2' THEN
             DERIV( Temp, XDot )
           ELSE
             PDeriv( Time+DtDay,Temp,DerivType,Order,BC,  XDot );

         TempTime:= Time + DtDay*0.5;

         { -------------- Evaluate 6th Taylor Series Term ----------- }
         FOR j:= 1 to 6 DO
           BEGIN
             ASSIGNVAL( K,   J,5,  DtTU*GETVAL( XDot,J,1) );
             ASSIGNVAL( Temp,J,1,  GETVAL( X,J,1 ) - GETVAL( K,J,1 )*8.0/27.0
                   + GETVAL( K,J,2 )* 2.0 - GETVAL( K,J,3 )*3544.0/2565.0
                   + GETVAL( K,J,4 )*1859.0/4104.0 - GETVAL( K,J,5 )*0.275 );
           END;
         IF DerivType[10] = '2' THEN
             DERIV( Temp, XDot )
           ELSE
             PDeriv( TempTime,Temp,DerivType,Order,BC,  XDot );

         FOR j:= 1 to 6 DO
             ASSIGNVAL( K,   J,6,  DtTU*GETVAL( XDot,J,1) );

         { -------------------- Check for convergence --------------- }
         Err:= 0.0;
         FOR j:= 1 to 6 DO
             Err:= ABS( GETVAL( K,J,1 )*1.0/360.0
                   - GETVAL( K,J,3 )*128.0/4275.0 - GETVAL( K,J,4 )*2197.0/75240.0
                   + GETVAL( K,J,5 )*0.02 + GETVAL( K,J,6 )*2.0/55.0 );

         { ------ Update the State vector, perform integration ------ }
         IF ( Err < Small ) or ( DtDay <= 2.0*HMin+Small ) THEN
           BEGIN
             FOR j:= 1 to 6 DO
                    ASSIGNVAL( X,J,1,  GETVAL( X,J,1 )
                    + GETVAL( K,J,1 )*25.0/216.0 + GETVAL( K,J,3 )*1408.0/2565.0
                    + GETVAL( K,J,4 )*2197.0/4104.0 - GETVAL( K,J,5 )*0.2 );
             Time:= Time + DtDay;
             s   := 0.0;
             Ktr := 1;
           END
           ELSE
           BEGIN
             S:= 0.84*POWER( Small*DtDay/Err, 0.25 );
             IF ( S < 0.75 ) and (DtDay > 2.0*HMin ) THEN   { Reduce  Step  Size }
                 DtDay:= DtDay * 0.5;
             IF ( S > 1.5 ) and (2.0*DtDay < HMax ) THEN    { Increase Step Size }
                 DtDay:= DtDay * 2.0;
             INC( Ktr );
 writeln( 'itime ',itime:18:11,ktr:3,'DtDay' ,DtDay:18:15,' err',err:10:7,' s',s:10:7,'kj6',GETVAL(k,1,6):10:6 );
 writeln( FileOut,'itime ',itime:18:11,ktr:3,'DtDay' ,DtDay:8:4,' err',err:10:7,' s',s:10:7,'kj6',GETVAL(k,1,6):10:6 );
           END;

       END;  { While }

     DELMATRIX( K );
     DELMATRIX( Temp );
   END;  { PROCEDURE RKF45 }
{}
{ ------------------------------------------------------------------------------
|
|                                PROCEDURE COWELL
|
|  This PROCEDURE uses a fourth order Runge-Kutta integrator on a 6 dimension
|    First Order differential equation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - Initial position vector        ER
|    V           - Initial velocity vector        ER/TU
|    ITime       - Initial Time (Julian Date)     Days from 4713 BC
|    FTime       - Final Time (Julian Date)       Days from 4713 BC
|    DtDay       - Step size                      Day
|    DerivType   - String of which perts to incl  'Y' and 'N'
|                  Options are in order : J2, J3,
|                  J4, SUN, MOON, Drag, Solarrad
|    BC          - Ballistic Coefficient          kg/m2
|
|  Outputs       :
|    R1          - Final position vector          ER
|    V1          - Final velocity vector          ER/TU
|
|  Locals        :
|    Time        - Current time during the loop   Days from 4713 BC
|    X           - State vector at each time      ER, ER/TU
|
|  Coupling      :
|    RK4         - Runge-Kutta algorithm
|    MAG         - Magnitude of a vector
|    INITMATRIX  - Initialize a matrix and fil with 0.0's
|    ASSIGNVAL   - Assign a value to a matrix
|    GETVAL      - Get a value from a matrix
|    DELMATRIX   - Delete a matrix
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE COWELL             ( R,V                                   : Vector;
                               ITime,FTime,DtDay                     : EXTENDED;
                               DerivType                             : Str10;
                               Order                                 : INTEGER;
                               BC                                    : EXTENDED;
                               VAR R1,V1                             : Vector   );
   CONST
     RadiusEarthKm : EXTENDED =  6378.1363;
     VKmPerSec     : EXTENDED =  7.905366149846;
   VAR
     i      : INTEGER;
     X,XDot : Matrix;
     Time   : EXTENDED;
   BEGIN
     x   := NIL;
     XDot:= NIL; { local }
     INITMATRIX( 6,1,1,1, X  );
     INITMATRIX( 6,1,1,1, XDot  );

     FOR i:= 1 to 6 DO
         IF i <= 3 THEN
             ASSIGNVAL( X,i,1,r[i] )
           ELSE
             ASSIGNVAL( X,i,1,v[i-3] );

     { ---------- Loop through the time interval desired ------------ }
     Time:= ITime;
     IF Show = 'F' THEN
         WriteLn( FileOut,(Time-ITime)*1440.0:8:3,Getval(x,1,1)*RadiusEarthKm:12:5,
                  Getval(x,2,1)*RadiusEarthKm:12:5,
                  Getval(x,3,1)*RadiusEarthKm:12:5,
                  Getval(x,4,1)*VKmPerSec:11:7,
                  Getval(x,5,1)*VKmPerSec:11:7,
                  Getval(x,6,1)*VKmPerSec:11:7 );

     { ---- This would be more accurate with ------------------------
               Julian Date,
               Secs from beginning of day
               Decimal seconds
       -------------------------------------------------------------- }

     WHILE Time <= FTime DO
       BEGIN
         IF Time+DtDay > FTime THEN
           BEGIN
             DtDay := FTime - Time;
             FTime := FTime - 1.0;
             WriteLn( 'Fixing end time dt is now  ',dtday*1440.0:11:6,' min ' );
           END;

         RK4( Time,DtDay,XDot,DerivType,Order,BC, X );
{         RKF45( Time,DtDay,XDot,DerivType,Order,BC, X );}

         Time := Time + DtDay;
         IF Show = 'F' THEN
             WriteLn( FileOut,(Time-ITime)*1440.0:8:3,Getval(x,1,1)*RadiusEarthKm:12:5,
                      Getval(x,2,1)*RadiusEarthKm:12:5,
                      Getval(x,3,1)*RadiusEarthKm:12:5,
                      Getval(x,4,1)*VKmPerSec:11:7,
                      Getval(x,5,1)*VKmPerSec:11:7,
                      Getval(x,6,1)*VKmPerSec:11:7 );

       END;  { While }

     { ------------------ Update the state vector ------------------- }
     FOR i:= 1 to 6 DO
         IF i <= 3 THEN
             r1[i]:= GETVAL( X,i,1 )
           ELSE
             v1[i-3]:= GETVAL( X,i,1 );

     MAG( R1 );
     MAG( V1 );

     DELMATRIX( X );
     DELMATRIX( XDot );

   END;  { PROCEDURE COWELL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ATMOS
|
|  This PROCEDURE finds the atmospheric density at an altitude above an
|    oblate earth given the position vector in the Geocentric Equatorial
|    frame.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - IJK Position vector            ER
|
|  Outputs       :
|    Rho         - Density                        kg/m**3
|
|  Locals        :
|    Hellp       - Height above ellipsoid         ER
|    OldDelta    - Previous value of DeltaLat     rad
|    Latgd       - Geodetic Latitude              -Pi/2 to Pi/2 rad
|    SinTemp     - Sine of Temp
|    RhoNom      - Nominal density at particular alt      gm/cm**3
|    NextBaseAlt - Next Base Altitude
|    LastBaseAlt - Last Base Altitude
|    H           - Scale Height                   km
|    i           - index
|    AtmosFile   - File of data for the
|                    exponential atmosphere
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ASIN      - Arc sine FUNCTION
|
|  References    :
|    Vallado       2007, 562, Ex 8-4
|
 ----------------------------------------------------------------------------- }

PROCEDURE ATMOS              ( R                                     : Vector;
                               VAR Rho                               : EXTENDED );
   CONST
     Small      : EXTENDED =     0.0000001;
     EE2Sqrd    : EXTENDED =     0.006694385000;
   VAR
     i : INTEGER;
     AtmosFile : TEXT;
     Hellp, OldDelta, Latgd, SinTemp, c, Decl, Temp, H, RhoNom, NextBaseAlt, LastBaseAlt  : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     ASSIGN( AtmosFile,'Atmosexp.dat' );
     RESET( AtmosFile );
     MAG( R );
     Decl := ASIN( R[3] / r[4] );
     Latgd:= Decl;

     { ----- Iterate to find Geocentric and Geodetic Latitude  ------ }
     Temp := SQRT( R[1]*R[1] + R[2]*R[2] );
     i:= 1;
     REPEAT
         OldDelta:= Latgd;
         SinTemp := SIN( Latgd );
         c       := 1.0 / (SQRT( 1.0-EE2Sqrd*SinTemp*SinTemp ));
         Latgd   := ARCTAN( (r[3]+c*EE2Sqrd*SinTemp)/Temp );
         INC( i );
     UNTIL ( ABS( OldDelta - Latgd ) < Small ) or ( i >= 10 );
     Hellp := ( (Temp/COS(Latgd)) - c ) * 6378.1363;

     IF i >= 10 THEN
         WriteLn( 'IJK2LL did NOT converge ' );

     { ----------- Determine density based on altitude -------------- }
     READ( AtmosFile, NextBaseAlt );
     REPEAT
         LastBaseAlt:= NextBaseAlt;
         ReadLn( AtmosFile, RhoNom,H);
         Read( AtmosFile,NextBaseAlt );
     UNTIL (NextBaseAlt >= Hellp) or (EOF(AtmosFile));

     RHO   := RHONOM*EXP((LastBaseAlt-Hellp)/H);
     CLOSE( AtmosFile );
   END;  { PROCEDURE ATMOS }


BEGIN

END.  { Unit AstPert }
{}
