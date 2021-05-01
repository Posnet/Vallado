{}
{      -----------------------------------------------------------------

                               TESTASTF.PAS

|   This file contains sample subroutines to test each of the functions
|   in the astrodynamic libraries.
|
|                                                                            }
(*                           Companion code for                              *)
(*             Fundamentals of Astrodyanmics and Applications                *)
(*                                  2007                                     *)
(*                            by David Vallado                               *)
(*                                                                           *)
(*     (W) 719-573-2600, email dvallado@agi.com                              *)
(*                                                                           *)
(*     *****************************************************************     *)
(*                                                                           *)
(*  Current :                                                                *)
(*            28 Jan 04  David Vallado                                       *)
(*                         Update headers                                    *)
(*  Changes :                                                                *)
(*            14 May 01  David Vallado                                       *)
(*                         2nd edition baseline                              *)
(*            23 Nov 87  David Vallado                                       *)
(*                         Original baseline                                 *)

{       ----------------------------------------------------------------- }

   uses newdelay,astutil,astmath,asttime,ast2body,astiod,astreduc,consts;

VAR
    InFile,OutFile : TEXT;
    i, NumChose : INTEGER;
    IFileName : Str12;
    Blank4    : String[4];
    Title     : String[15];


{}
{ ------------------------------------------------------------------------
|
|                       Individual PROCEDURES
|
|  These subroutines implement each function or procedure using a datafile
|    with the number of each routine to identify which subroutine to use.
|
|
  ------------------------------------------------------------------------ }

      PROCEDURE TestGETPART;
      VAR
        i, LocStart,Leng : INTEGER;
        InStr : Str250;

      BEGIN
        ReadLn(InFile, InStr );
        WriteLn( OutFile,  InStr );
        ReadLn(InFile, LocStart,Leng );
        WriteLn( OutFile,  LocStart,Leng );

        i := GETPART   ( InStr, LocStart,Leng );

        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, i );

      END;

      PROCEDURE TestGETPARTL;
      VAR
        LocStart,Leng : INTEGER;
        GL : INTEGER;
        InStr : Str250;

      BEGIN
        ReadLn( InFile, InStr );
        WriteLn( OutFile,  InStr );
        ReadLn( InFile, LocStart,Leng );
        WriteLn( OutFile,  LocStart,Leng );

        Gl := GETPARTL( InStr, LocStart,Leng );

        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Gl );

      END;

      PROCEDURE TestGetPartR;
      VAR
        LocStart,Leng : INTEGER;
        GR : EXTENDED;
        InStr : Str250;

      BEGIN
        ReadLn( InFile, InStr);
        WriteLn( OutFile,  InStr );
        ReadLn( InFile, LocStart,Leng );
        WriteLn( OutFile,  LocStart,Leng );
 
        GR := GetPartR   ( InStr, LocStart,Leng );

        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, GR );
 
      END;
 
      PROCEDURE TestFACTORIAL;
      VAR
        x : INTEGER;
        Fact : Extended;
 
      BEGIN
        ReadLn( InFile, x );
 
        Fact := FACTORIAL( x );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Fact );
 
      END;
 
      PROCEDURE TestBINOMIAL;
      VAR
        i,j : INTEGER;
        Bio : Extended;
 
      BEGIN
        ReadLn( InFile, i,j );
        WriteLn( OutFile,  i,j );
 
        Bio := BINOMIAL ( i,j );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Bio );
 
      END;
 
      PROCEDURE TestMIN;
      VAR
        Mn, x, y : extended;
 
      BEGIN
        ReadLn( InFile, x,y );
        WriteLn( OutFile,  x,y );
 
        Mn := MIN      ( X, Y );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Mn );
 
      END;
 
      PROCEDURE TestMAX;
      VAR
        Mx, x, y : Extended;
 
      BEGIN
        ReadLn( InFile, x,y );
        WriteLn( OutFile,  x,y );
 
        Mx := MAX      ( X, Y );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Mx );
 
      END;
 
      PROCEDURE TestPLANE;
      VAR
        x1,y1,z1,x2,y2,z2,x3,y3,z3,a,b,c,d : extended;
 
      BEGIN
        ReadLn( InFile, x1,y1,z1 );
        WriteLn( OutFile,  x1,y1,z1 );
        ReadLn( InFile, x2,y2,z2 );
        WriteLn( OutFile,  x2,y2,z2 );
        ReadLn( InFile, x3,y3,z3 );
        WriteLn( OutFile,  x3,y3,z3 );
 
        PLANE         ( x1,y1,z1,x2,y2,z2,x3,y3,z3, a,b,c,d );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, a,b,c,d );
 
      END;
 
      PROCEDURE TestACOSH;
      VAR
         XVal, AC : extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
 
        AC := ACOSH( XVal );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, AC );
 
      END;
 
      PROCEDURE TestSINH;
      VAR
         XVal, S : Extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
 
        S := SINH   ( XVal );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, S );
 
      END;
 
      PROCEDURE TestASINH;
      VAR
         XVal, AS : Extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
 
        AS := ASINH( XVal );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, AS );
 
      END;
 
      PROCEDURE TestATANH;
      VAR
         XVal, AT : Extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
 
        AT := ATANH( XVal );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, AT );
 
      END;
 
      PROCEDURE TestDOT;
      VAR
         Dt : Extended;
         Vec1,Vec2 : Vector;
 
      BEGIN
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
 
        Dt := DOT    ( Vec1,Vec2 );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Dt );
 
      END;
 
      PROCEDURE TestCROSS;
      VAR
         Vec1,Vec2, OutVec: vector;
 
      BEGIN
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
 
        CROSS       ( Vec1,Vec2, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestMAG;
      VAR
         Vec : vector;
 
      BEGIN
        ReadLn( InFile, Vec[1],Vec[2],Vec[3] );
        WriteLn( OutFile,  Vec[1],Vec[2],Vec[3] );
 
        MAG         ( Vec );
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Vec[4] );
 
      END;
 
      PROCEDURE TestNORM;
      VAR
         Vec, OutVec: vector;
 
      BEGIN
        ReadLn( InFile, Vec[1],Vec[2],Vec[3] );
        WriteLn( OutFile,  Vec[1],Vec[2],Vec[3] );
 
        NORM        ( Vec, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestROT1;
      VAR
         Vec, OutVec:vector;
         XVal:extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
        ReadLn( InFile, Vec[1],Vec[2],Vec[3] );
        WriteLn( OutFile,  Vec[1],Vec[2],Vec[3] );
 
        MAG( Vec );
        ROT1        ( Vec, XVal, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestROT2;
      VAR
         Vec, OutVec : vector;
         XVal : Extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
        ReadLn( InFile, Vec[1],Vec[2],Vec[3] );
        WriteLn( OutFile,  Vec[1],Vec[2],Vec[3] );
 
        MAG( Vec );
        ROT2        ( Vec, XVal, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestROT3;
      VAR
         Vec, OutVec : vector;
         XVal : Extended;
 
      BEGIN
        ReadLn( InFile, XVal );
        WriteLn( OutFile,  XVal );
        ReadLn( InFile, Vec[1],Vec[2],Vec[3] );
        WriteLn( OutFile,  Vec[1],Vec[2],Vec[3] );
 
        MAG( Vec );
        ROT3        ( Vec, XVal, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestADDVEC;
      VAR
         Vec1,Vec2, OutVec : vector;
 
      BEGIN
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
        MAG( Vec1 );
        MAG( Vec2 );
 
        ADDVEC      ( Vec1,Vec2, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestADD3VEC;
      VAR
         Vec1, Vec2, Vec3, OutVec:vector;
 
      BEGIN
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
        ReadLn( InFile, Vec3[1],Vec3[2],Vec3[3] );
        WriteLn( OutFile,  Vec3[1],Vec3[2],Vec3[3] );
        MAG( Vec1 );
        MAG( Vec2 );
        MAG( Vec3 );
 
        ADD3VEC     ( Vec1,Vec2,Vec3, OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestLNCOM1;
      VAR
         Vec, OutVec:vector;
         a : extended;
 
      BEGIN
        ReadLn( InFile, a );
        WriteLn( OutFile,  a );
        ReadLn( InFile, Vec[1],Vec[2],Vec[3] );
        WriteLn( OutFile,  Vec[1],Vec[2],Vec[3] );
 
        LNCOM1      ( A, Vec,  OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestLNCOM2;
      VAR
         Vec1, Vec2,OutVec :vector;
         A1, A2: extended;
 
      BEGIN
        ReadLn( InFile, A1,A2 );
        WriteLn( OutFile,  A1,A2 );
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
        MAG( Vec1 );
        MAG( Vec2 );
 
        LNCOM2      ( A1, A2, Vec1, Vec2,  OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestLNCOM3;
      VAR
         Vec1, Vec2, Vec3, OutVec :vector;
         A1, A2, A3: extended;
 
      BEGIN
        ReadLn( InFile, A1,A2, A3 );
        WriteLn( OutFile,  A1,A2, A3 );
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
        ReadLn( InFile, Vec3[1],Vec3[2],Vec3[3] );
        WriteLn( OutFile,  Vec3[1],Vec3[2],Vec3[3] );
        MAG( Vec1 );
        MAG( Vec2 );
        MAG( Vec3 );
 
        LNCOM3      ( A1, A2, A3, Vec1, Vec2, Vec3,  OutVec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, OutVec[1],OutVec[2],OutVec[3] );
 
      END;
 
      PROCEDURE TestPOLYFIT;
      VAR
        Degree, NumPts, i : INTEGER;
        DataPoints,Coeff : matrix;
        a,b,MinX, MinY : Extended;
 
      BEGIN
        InitMatrix( 10,2, 1,1, datapoints ) ;
        ReadLn( InFile, Degree, Numpts );
        WriteLn( OutFile,  Degree, Numpts );
        FOR i := 1 to NumPts DO
          BEGIN
            ReadLn( InFile, a, b );
{            ReadLn( InFile, datapoints[i,1], DataPoints[i,2] );
            WriteLn( OutFile,  datapoints[i,1], DataPoints[i,2] );}
          END;
 
        POLYFIT     ( Degree,NumPts,DataPoints,Coeff,MinX,MinY );
 
        WriteLn( OutFile, '  Results:' );
        For i:=1 to Degree DO
          BEGIN
{            WriteLn( OutFile, Coeff[i,1] );}
          END;
        WriteLn( OutFile, Minx, MinY );
 
      END;
 
      PROCEDURE TestANGLE;
      VAR
         Vec1, Vec2 : Vector;
         Theta : Extended;
 
      BEGIN
        ReadLn( InFile, Vec1[1],Vec1[2],Vec1[3] );
        WriteLn( OutFile,  Vec1[1],Vec1[2],Vec1[3] );
        ReadLn( InFile, Vec2[1],Vec2[2],Vec2[3] );
        WriteLn( OutFile,  Vec2[1],Vec2[2],Vec2[3] );
 
        ANGLE       ( Vec1,Vec2, Theta );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Theta );
 
      END;
 
      PROCEDURE TestFACTOR;
      VAR
         Poly, Roots : matrix;
        NRootS,i : INTEGER;
 
      BEGIN
        ReadLn( InFile, NRoots );
        WriteLn( OutFile,  NRoots );
        FOR i:=1 to NRoots+1 DO
          BEGIN
{            ReadLn( InFile, Poly(i) );
            WriteLn( OutFile, Poly(i) );}
          END;  { For  }
 
{        FACTOR      ( Poly, NRootS, RootS );}
 
        WriteLn( OutFile, '  Results:' );
        FOR i := 1 to Nroots DO
          BEGIN
{            WriteLn( OutFile, Roots(i,1),Roots(i,2) );}
          END;  { For  }
 
      END;
 
      PROCEDURE TestQUADRATIC;
      VAR
         R1r,R1i,R2r,R2i,a,b,c : Extended;
 
      BEGIN
        ReadLn( InFile, a,b,c );
        WriteLn( OutFile,  a,b,c );
 
        QUADRATIC   ( a,b,c, R1r,R1i,R2r,R2i );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, R1r,R1i,R2r,R2i );
 
      END;
 
      PROCEDURE TestCUBIC;
      VAR
         R1r,R1i,R2r,R2i,R3r,R3i,a,b,c,d : Extended;
 
      BEGIN
        ReadLn( InFile, a,b,c,d );
        WriteLn( OutFile,  a,b,c,d );
 
        CUBIC       ( a,b,c,d, R1r,R1i,R2r,R2i,R3r,R3i );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, R1r,R1i,R2r,R2i,R3r,R3i );
 
      END;
 
      PROCEDURE TestQUARTIC;
      VAR
         R1r,R1i,R2r,R2i,R3r,R3i,R4r,R4i,a,b,c,d,e : Extended;
 
      BEGIN
        ReadLn( InFile, a,b,c,d,e );
        WriteLn( OutFile,  a,b,c,d,e );
 
        QUARTIC     ( a,b,c,d,e, R1r,R1i,R2r,R2i,
                               R3r,R3i,R4r,R4i );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, R1r,R1i,R2r,R2i,R3r,R3i,R4r,R4i );
 
      END;
 
      PROCEDURE TestMATSCALE;
      VAR
        Mat1r,Mat1c :INTEGER ;
         Mat1,Mat2 : matrix;
         Scale : Extended;
 
      BEGIN
        ReadLn( InFile, Mat1r,Mat1c,Scale );
        WriteLn( OutFile,  Mat1r,Mat1c,Scale );
{        ReadLnMat( Mat1,Mat1r,Mat1c ) );}
 
{        MATSCALE    ( Mat1,Scale,Mat1r,Mat1c, Mat2 );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( Mat2(Mat1r,Mat1c),'Ans' );}
 
      END;
 
      PROCEDURE TestMATMULT;
      VAR
         Mat1r,Mat1c,Mat2c : INTEGER;
         Mat1,Mat2,Mat3 : matrix;
 
      BEGIN
        ReadLn( InFile, Mat1r,Mat1c,Mat2c );
        WriteLn( OutFile,  Mat1r,Mat1c,Mat2c );
{        ReadLnMat( Mat1,Mat1r,Mat1c );
        ReadLnMat( Mat2,Mat1c,Mat2c );}
 
{        MATMULT     ( Mat1,Mat2, Mat1r,Mat1c,Mat2c, Mat3 );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( Mat3,Mat1r,Mat2c,'Ans' );}
 
      END;
 
      PROCEDURE TestMATADD;
      VAR
        Mat1r,Mat1c: INTEGER ;
         Mat1,Mat2,Mat3 : matrix;
 
      BEGIN
        ReadLn( InFile, Mat1r,Mat1c );
        WriteLn( OutFile,  Mat1r,Mat1c );
{        ReadLnMat( Mat1,Mat1r,Mat1c );
        ReadLnMat( Mat2,Mat1r,Mat1c );}
 
{        MATADD      ( Mat1,Mat2, Mat1r,Mat1c, Mat3 );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( Mat3,Mat1r,Mat1c,'Ans' );}
 
      END;
 
      PROCEDURE TestMATSUB;
      VAR
        Mat1r,Mat1c :INTEGER ;
         Mat1,Mat2,Mat3 : matrix;
 
      BEGIN
        ReadLn( InFile, Mat1r, Mat1c );
        WriteLn( OutFile,  Mat1r, Mat1c );
{        ReadLnMat( Mat1,Mat1r,Mat1c );
        ReadLnMat( Mat2,Mat1r,Mat1c );
 
        MATSUB      ( Mat1,Mat2, Mat1r,Mat1c, Mat3 );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( Mat3,Mat1r,Mat1c,'Ans' );}
 
      END;
 
      PROCEDURE TestMATTRANS;
      VAR
        Mat1r,Mat1c :INTEGER ;
         Mat1,Mat2 : matrix;
 
      BEGIN
        ReadLn( InFile, Mat1r, Mat1c );
        WriteLn( OutFile,  Mat1r, Mat1c );
{        ReadLnMat( Mat1,Mat1r,Mat1c );}
 
{        MATTRANS    ( Mat1, Mat1r,Mat1c, Mat2 );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( Mat2,Mat1r,Mat1c,'Ans' );}
 
      END;
 
      PROCEDURE TestMAKEMAT;
      VAR
         Angl, Matr : Extended;
        Numbr: INTEGER ;
 
      BEGIN
        ReadLn( InFile, Angl,Numbr );
        WriteLn( OutFile, Angl,Numbr );
 
{        MAKEMAT     ( Angl, Numbr, Matr );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( Matr,3,3,'Ans' );}
 
      END;
 
      PROCEDURE TestMATINVERSE;
      VAR
        Order : INTEGER ;
         Mat, MatInv:matrix;
 
      BEGIN
        ReadLn( InFile, Order);
        WriteLn( OutFile,  Order );
{        ReadLnMat( Mat,Order,Order );}
 
{        MATINVERSE  ( Mat, Order, MatInv );}
 
        WriteLn( OutFile, '  Results:' );
{        FilePrintMat( MatInv,Order,Order,'Ans' );}
 
      END;
 
      PROCEDURE TestPRINTMAT;
      VAR
         Mat1r,Mat1c : INTEGER;
         Mat1: matrix;
         Title: STRING[64];
 
      BEGIN
        ReadLn( InFile, Mat1r,Mat1c );
        WriteLn( OutFile,  Mat1r,Mat1c );
{        ReadLnMat( Mat1,Mat1r,Mat1c );}
 
{        PRINTMAT    ( Mat1, Mat1r,Mat1c,Title );}
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile );
 
      END;
 
      PROCEDURE TestFILEPRINTMAT;
      VAR
         Mat1r,Mat1c : INTEGER;
         Mat1:matrix;
         Title : Str64;
 
      BEGIN
        ReadLn( InFile, Mat1r,Mat1c );
        WriteLn( OutFile,  Mat1r,Mat1c );
 
{        FILEPRINTMAT( Mat1, Mat1r,Mat1c,Title );}
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile );
 
      END;
 
      PROCEDURE TestFILEEXPPRINTMAT;
      VAR
         Mat1r,Mat1c : INTEGER;
         Mat1:matrix;
         Title : Str64;
 
      BEGIN
        ReadLn( InFile, Mat1r,Mat1c );
        WriteLn( OutFile,  Mat1r,Mat1c );
 
{        FILEEXPPRINTMAT ( Mat1,Mat1r,Mat1c,Title );}
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile );
 
      END;
 
      PROCEDURE TestDETERMINANT;
      VAR
         Order : INTEGER ;
         Det : Extended;
         Mat1 : Matrix;
 
      BEGIN
        ReadLn( InFile, Order );
        WriteLn( OutFile,  Order );
{        ReadLnMat( Mat1,Order,Order );}
 
        Det := DETERMINANT( Mat1, Order );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Det );
 
      END;
 
   { -------------------- Time s -----------------------------------;
 
      PROCEDURE TestUpCaseSt;
      VAR
        S : str250;
        UpCaseSt, UpS : Str250;
 
      BEGIN
        ReadLn( InFile, S );
        WriteLn( OutFile,  S );
 
        UpS := UpCaseSt ( S );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Ups );
 
      END;
 
 
      PROCEDURE TestINITTIME;
 
      BEGIN
        ReadLn( InFile, );
        WriteLn( OutFile, 'In ' );
 
        INITTIME;
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, );
 
      END;
 
      PROCEDURE TestGETINTMON;
      VAR
        MonStr : Str3;
        Month, GetintMon:INTEGER ;
 
      BEGIN
        ReadLn( InFile, monstr );
        WriteLn( OutFile,  monstr );
 
        Month := GETINTMON  ( MonStr );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, monstr,month );
 
      END;
 
      PROCEDURE TestGETINTDAY;
      VAR
         DayStr : str3;
        DayVal : INTEGER;
 
      BEGIN
        ReadLn( InFile, DayStr );
        WriteLn( OutFile,  DayStr );
 
        DayVal := GETINTDAY  ( DayStr );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, DayVal );
 
      END;
 
      PROCEDURE TestDAYOFWEEK;
      VAR
         JD : Extended;
         DayW, DAYOFWEEK : INTEGER;
 
      BEGIN
        ReadLn( InFile, JD );
        WriteLn( OutFile,  JD );
 
        DayW := DAYOFWEEK  ( JD );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, DayW );
 
      END;
 
 
      PROCEDURE TestDAYLIGHTST;
      VAR
         Year, StartDay, StopDay : INTEGER ;
         JDStartDST, JDStopDST : Extended;
 
      BEGIN
        ReadLn( InFile, Year );
        WriteLn( OutFile,  Year );
 
        DAYLIGHTST  ( Year, StartDay, StopDay, JDStartDST,;
                               JDStopDST );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, StartDay, StopDay, JDStartDST, JDStopDST );
 
      END;
 
      PROCEDURE TestJULIANDAY;
      VAR
        Year, Mon, Day, Hr, Min:INTEGER ;
          Sec, JD : Extended;
 
      BEGIN
        ReadLn( InFile, Year, Mon, Day, Hr, Min, Sec );
        WriteLn( OutFile,  Year, Mon, Day, Hr, Min, Sec );
 
        JULIANDAY   ( Year,Mon,Day,Hr,Min, Sec, JD );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, JD );
 
      END;
 
      PROCEDURE TestJULIANDAYALL;
      VAR
        Year, Mon, Day, Hr, Min:INTEGER ;
        WhichType : char;
          Sec, JD : Extended;
 
      BEGIN
        ReadLn( InFile, Year, Mon, Day, Hr, Min, Sec,  WhichType );
        WriteLn( OutFile,  Year, Mon, Day, Hr, Min, Sec, WhichType );
 
        JULIANDAYALL( Year,Mon,Day,Hr,Min,Sec, WhichType, JD );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, JD );
 
      END;
 
      PROCEDURE TestDAYS2MDHMS;
      VAR
         Days,Sec : Extended;
        Year, Mon, Day, Hr, Min:INTEGER ;
 
      BEGIN
        ReadLn( InFile, Year, Days );
        WriteLn( OutFile,  Year, Days );
 
        DAYS2MDHMS   ( Year,Days,  Mon,Day,Hr,Min,Sec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Mon,Day,Hr,Min,Sec );
 
      END;
 
      PROCEDURE TestINVJULIANDAY;
      VAR
        Year, Mon, Day, Hr, Min:INTEGER ;
          Sec, JD : Extended;
 
      BEGIN
        ReadLn( InFile, JD );
        WriteLn( OutFile,  JD );
 
        INVJULIANDAY( JD, Year,Mon,Day,Hr,Min, Sec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Year,Mon,Day,Hr,Min,Sec );
 
      END;
 
      PROCEDURE TestFINDDAYS;
      VAR
        Year, Month, Day, Hr, Min:INTEGER ;
         Sec, Days : Extended;
 
      BEGIN
        ReadLn( InFile, year,Month,Day,Hr,Min,Sec );
        WriteLn( OutFile,  year,Month,Day,Hr,Min,Sec );
 
        FINDDAYS    ( Year,Month,Day,Hr,Min, Sec,  Days );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Days );
 
      END;
 
      PROCEDURE TestLSTIME;
      VAR
         Lon, JD, LST, GST,Rad : Extended;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, Lon,JD );
        WriteLn( OutFile,  Lon,JD );
        Lon := Lon / Rad;
 
        LSTIME      ( Lon, JD, LST, GST );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, LST*rad,GST*rad );
 
      END;
 
      PROCEDURE TestSUNRISESET;
      VAR
         JD, Latgd, Lon, UTSunRise, UTSunSet, Rad : Extended;
        WhichKind : char;
         Error:Str12;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, JD, Latgd, Lon, Whichkind );
        WriteLn( OutFile,  JD, Latgd, Lon,  WhichKind );
        Latgd:= Latgd / Rad;
        Lon := Lon / Rad;
 
        SUNRISESET  ( JD,Latgd,Lon, WhichKind, UTSunRise,;
                               UTSunSet, Error );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, UTSunRise, UTSunSet, Error );
 
      END;
 
      PROCEDURE TestMOONRISESET;
      VAR
         JD, Latgd, Lon, UTMoonRise, UTMoonSet, MoonPhaseAng,Rad : Extended;
         Error: str12;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, JD, Latgd, Lon );
        WriteLn( OutFile,  JD, Latgd, Lon );
        Latgd:= Latgd / Rad;
        Lon := Lon / Rad;
 
        MOONRISESET ( JD,Latgd,Lon, UTMoonRise, UTMoonSet,;
                               MoonPhaseAng, Error );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, UTMoonRise, UTMoonSet, MoonPhaseAng, Error );
 
      END;
 
      PROCEDURE TestHMS_SEC;
      VAR
        Hr, Min:INTEGER ;
          Sec, UTSec : Extended;
         Direction: str4;
 
      BEGIN
        ReadLn( InFile, Hr, Min, Sec, Direction );
        WriteLn( OutFile,  Hr, Min, Sec,':', Direction,':' );
 
        HMS_SEC     ( Hr,Min, Sec, Direction, UTSec );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, UTSec );
 
      END;
 
      PROCEDURE TestHMS_UT;
      VAR
        Hr, Min:INTEGER ;
          Sec, UT : Extended;
         Direction: str4;
 
      BEGIN
        ReadLn( InFile, Hr, Min, Sec, Direction );
        WriteLn( OutFile,  Hr, Min, Sec,  Direction );
 
        HMS_UT      ( Hr,Min, Sec, Direction, UT );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, UT );
 
      END;
 
      PROCEDURE TestHMS_RAD;
      VAR
        Hr, Min: INTEGER ;
          Sec, HMS : Extended;
         Direction: str4;
 
      BEGIN
        ReadLn( InFile, Hr, Min, Sec, Direction );
        WriteLn( OutFile,  Hr, Min, Sec,  Direction );
 
        HMS_RAD     ( Hr,Min, Sec, Direction, HMS );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, HMS );
 
      END;
 
      PROCEDURE TestDMS_RAD;
      VAR
         Deg, Min: INTEGER;
         Sec, DMS, Rad : Extended;
         Direction: str4;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, Deg, Min, Sec,  Direction );
        WriteLn( OutFile,  Deg, Min, Sec,  Direction );
        Deg := Deg / Rad;
 
        DMS_RAD     ( Deg,Min, Sec, Direction, DMS );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, DMS );
 
      END;
 
 
 
   ! --------------------  2 body s --------------------------------;
      PROCEDURE TestELORB;
      VAR
         R,V: vector;
         P,A,Ecc,Incl,Omega,Argp,Nu,M,ArgLat,TrueLon,
               LonPer,Rad : Extended;

      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, R[1],R[2],R[3] );
        WriteLn( OutFile,  R[1],R[2],R[3] );
        ReadLn( InFile, V[1],V[2],V[3] );
        WriteLn( OutFile,  V[1],V[2],V[3] );
        MAG( R ) );
        MAG( V );
 
        ELORB( R,V,  P,A,Ecc,Incl,Omega,Argp,Nu,M,ArgLat,;
                    TrueLon,LonPer );
 
        WriteLn( OutFile, '  Results: ' );
        Write(20,'(3(f14.8))') P,A,Ecc );
        Write(20,'(5(f14.8))') Incl*rad,Omega*rad,Argp*rad,Nu*rad,M*rad );
        Write(20,'(3(f14.8))') ArgLat*rad, Truelon*rad, LonPer*rad );
 
      END;
 
      PROCEDURE TestRANDV;
      VAR
         R,V : vector;
         P,a,Ecc,Incl,Omega,Argp,Nu,ArgLat,TrueLon,
               LonPer, Rad : Extended;

      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, p,a,ecc,incl,Omega,Argp,Nu,ArgLat,TrueLon,LonPer );
        WriteLn( OutFile, p,a,ecc,incl,Omega,Argp,Nu,ArgLat,TrueLon, );
              LonPer );
        incl:= incl/Rad;
        Omega:= Omega/Rad;
        Argp:= Argp/Rad;
        Nu:= Nu/Rad;
        ArgLat:= ArgLat/Rad;
        TrueLon:= TrueLon/Rad;
        LonPer:= LonPer/Rad;
 
        RANDV( P,Ecc,Incl,Omega,Argp,Nu,ArgLat,TrueLon,LonPer,R,V);
 
        WriteLn( OutFile, '  Results:' );
        Write(20,'(4(f14.8))') R[1],R[2],R[3],R[4] );
        Write(20,'(4(f14.8))') V[1],V[2],V[3],V[4] );
 
      END;
 
      PROCEDURE TestFINDC2C3;
      VAR
         ZNew, C2New, C3New : Extended;
 
      BEGIN
        ReadLn( InFile, ZNew );
        WriteLn( OutFile,  ZNew );
 
        FINDC2C3    ( ZNew, C2New,C3New );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, C2New, C3New );
 
      END;
 
      PROCEDURE TestNEWTONE;
      VAR
         Ecc, E0, M, Nu, Rad : Extended;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, Ecc, E0 );
        WriteLn( OutFile,  Ecc, E0 );
        E0 := E0 / Rad;
 
        NEWTONE     ( Ecc,E0,  M,Nu );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, M*Rad, Nu*Rad );
 
      END;
 
      PROCEDURE TestNEWTONM;
      VAR
         Ecc, M, E0, Nu, Rad : Extended;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, Ecc, M;
        WriteLn( OutFile,  Ecc, M );
        M := M /Rad;
 
        NEWTONM     ( Ecc,M, E0,Nu );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, E0*Rad, Nu*Rad );
 
      END;
 
 
      PROCEDURE TestNEWTONNU;
      VAR
         Ecc, Nu, E0, M, Rad : Extended;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, Ecc, Nu );
        WriteLn( OutFile,  Ecc, Nu );
        Nu := Nu / Rad;
 
        NEWTONNU    ( Ecc,Nu, E0,M );
 
        WriteLn( OutFile, '  Results:' ) );
        WriteLn( OutFile, E0*Rad, M*Rad;
 
      END;
 
      PROCEDURE TestKEPLER;
      VAR
         Ro,Vo, R,V : vector;
         DtTU : extended;
         Error Str12;

      BEGIN
        ReadLn( InFile, Ro[1],Ro[2],Ro[3] );
        WriteLn( OutFile,  Ro[1],Ro[2],Ro[3] );
        ReadLn( InFile, Vo[1],Vo[2],Vo[3] );
        WriteLn( OutFile,  Vo[1],Vo[2],Vo[3] );
        ReadLn( InFile, DtTU );
        WriteLn( OutFile,  DtTU );
        MAG( Ro );
        MAG( Vo );
 
        KEPLER      ( Ro,Vo, DtTU, R,V, Error );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, R[1],R[2],R[3],R[4] );
        WriteLn( OutFile, V[1],V[2],V[3],V[4] );
 
      END;
 
      PROCEDURE TestFINDTOF;
      VAR
         Ro, R : vector;
         p, Tof : Extended;
      BEGIN
        ReadLn( InFile, Ro[1],Ro[2],Ro[3] );
        WriteLn( OutFile,  Ro[1],Ro[2],Ro[3] );
        ReadLn( InFile, R[1],R[2],R[3], p );
        WriteLn( OutFile,  R[1],R[2],R[3], p );
 
        FINDTOF     ( Ro,R, p, Tof );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, tof );
 
      END;
 
      PROCEDURE TestIJKTOLATLONA;
      VAR
         R : vector;
         JD, Latgc,Latgd,Lon,Hellp,Rad : Extended;
 
      BEGIN
        Rad := 57.29577951308230D0;
        ReadLn( InFile, r[1],r[2],r[3] );
        WriteLn( OutFile,  r[1],r[2],r[3] );
        MAG( r );
        ReadLn( InFile, JD;
 
        IJKTOLATLONA( R, JD, Latgc,Latgd,Lon,Hellp );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, LatGc*rad,LatGd*Rad,Lon*rad,Hellp );
 
      END;
 
      PROCEDURE TestGEOCGEOD;
      VAR
         Latgc,Latgd, rad : Extended;
        Direction: str4;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, Latgc, Direction );
        WriteLn( OutFile,  Latgc, Direction );
        Latgc := Latgc / rad;
 
        GEOCGEOD    ( Latgc, Direction, Latgd );
 
        WriteLn( OutFile, '  Results:' ) );
        WriteLn( OutFile, Latgd*rad );
 
      END;
 
      PROCEDURE TestSIGHT;
      VAR
         R1,R2:vector;
         WhichKind : char;
         LOS : str3;
 
      BEGIN
        ReadLn( InFile, r1[1],r1[2],r1[3] );
        WriteLn( OutFile,  r1[1],r1[2],r1[3] );
        MAG( r1 );
        ReadLn( InFile, r2[1],r2[2],r2[3],  WhichKind );
        WriteLn( OutFile,  r2[1],r2[2],r2[3],  WhichKind );
        MAG( r2 );
 
        SIGHT       ( R1,R2, WhichKind, LOS );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, LOS );
 
      END;
 
      PROCEDURE TestSUN;
      VAR
         JD,RSun[4], RtAsc, Decl : Extended;
 
      BEGIN
        ReadLn( InFile, JD );
        WriteLn( OutFile,  JD );
 
        SUN         ( JD, RSun, RtAsc,Decl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, RSun[1],RSun[2],RSun[3], RSun[4] );
        WriteLn( OutFile, RtAsc, Decl );
 
      END;
 
      PROCEDURE TestMOON;
      VAR
         JD,  RtAsc, Decl : Extended;
         RMoon: vector;
      BEGIN
        ReadLn( InFile, JD );
        WriteLn( OutFile,  JD );
 
        MOON        ( JD, RMoon, RtAsc,Decl );
 
        WriteLn( OutFile, '  Results:'  );
        WriteLn( OutFile, RMoon[1],RMoon[2],RMoon[3], RMoon[4] );
        WriteLn( OutFile, RtAsc, Decl );
 
      END;
 
      PROCEDURE TestLIGHT;
      VAR
         R ; vector;
         JD : Extended;
          WhichKind ; char;
          Lit : str3
 
      BEGIN
        ReadLn( InFile, r[1],r[2],r[3] );
        WriteLn( OutFile,  r[1],r[2],r[3] );
        MAG( r );
        ReadLn( InFile, JD,  WhichKind);
        WriteLn( OutFile,  JD,  WhichKind );
 
        LIGHT       ( R, JD, WhichKind, LIT );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, LIT );
 
      END;
 
      PROCEDURE TestCHECKHITEARTH;
      VAR
         RInt, V1t, RTgt,V2t:vector;
        HitEarth : char;
 
      BEGIN
        ReadLn( InFile, Rint[1] );
        WriteLn( OutFile,  Rint[1] );
 
        CHECKHITEARTH ( Rint,V1t,Rtgt,V2t, HitEarth );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, HitEarth );
 
      END;
 
      PROCEDURE TestSATFOV;
      VAR
         Incl, Az, SLatgd, SLon, SAlt, tFOV, EtaCtr, FovMAx,
           TotalRng, RhoMax, RhoMin,TgtLat, TgtLon, rad : Extended;
        Rad := 57.29577951308230D0;
 
 
      BEGIN
        ReadLn( InFile, Incl,Az,SLatgd,SLon,SAlt,tFOV,EtaCtr,FOVMax );
        WriteLn( OutFile,  Incl,Az,SLatgd,SLon,SAlt,tFOV,EtaCtr,FOVMax );
        Incl := Incl / rad;
        az := az / rad;
        slatgd:=slatgd / rad;
        slon:=slon/rad;
 
        SATFOV      ( Incl,Az, SLatgd, SLon, SAlt,tFOV,EtaCtr,;
                             FovMax, TotalRng, RhoMax, RhoMin,TgtLat,;
                             TgtLon );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, TotalRng, RhoMax, RhoMin, TgtLat,TgtLon );
 
      END;
 
      PROCEDURE TestRNGAZ;
      VAR
         LLat, LLon, TLat, TLon, Tof, Range, Az, rad : Extended;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, LLat,LLon,TLat,TLon );
        WriteLn( OutFile,  LLat,LLon,TLat,TLon );
        LLat := LLat / rad;
        LLon := LLon / rad;
        TLat := TLat / rad;
        TLon := TLon / rad;
 
        RNGAZ       ( LLat,LLon,TLat,TLon,Tof, Range, Az );
 
        WriteLn( OutFile, '  Results:' ) );
        WriteLn( OutFile, TOF,Range,Az*Rad );
 
 
      END;
 
      PROCEDURE TestPATH;
      VAR
         LLat, LLon, Range, Az, TLat, TLon, rad;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, LLat,LLon,Range,Az );
        WriteLn( OutFile,  LLat,LLon,Range,Az );
        LLat := LLat / rad;
        LLon := LLon / rad;
        Az := Az /rad;
 
        PATH        ( LLat, LLon, Range, Az, TLat, TLon );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, TLat*rad,TLon*rad );
 
      END;
 
   ! --------------------  IOD s -----------------------------------;
      PROCEDURE TestSITE;
      VAR
         LatGd,Alt,LST,Rad,ReFt : Extended;
         RS,VS : vector;
 
      BEGIN
        Rad := 57.29577951308230D0;
        Reft := 20925644.0288713D0;
        ReadLn( InFile, Latgd,Alt,LST );
        WriteLn( OutFile,  Latgd,Alt,LST );
        Latgd:= Latgd / Rad;
        Lst:= Lst / Rad;
        Alt:= Alt / REFt;
 
         SITE        ( Latgd,Alt,LST, RS,VS );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, RS[1],RS[2],RS[3],RS[4] );
        WriteLn( OutFile, VS[1],VS[2],VS[3],VS[4] );
 
      END;
 
      PROCEDURE TestANGLESLAPLACE;
      VAR
         Delta1,Delta2,Delta3,Alpha1,Alpha2,Alpha3,JD1,JD2,JD3 : Extended;
          RS1,RS2,RS3,r2,v2 : Vector;
 
 
      BEGIN
        ReadLn( InFile, Delta1,Delta2,Delta3,Alpha1,Alpha2,Alpha3,JD1,JD2,JD3);
        WriteLn( OutFile,  Delta1,Delta2,Delta3,Alpha1,Alpha2,Alpha3,JD1,
               JD2,JD3 );
 
         ANGLESLAPLACE( Delta1,Delta2,Delta3,Alpha1,Alpha2,
                                Alpha3,JD1,JD2,JD3,RS1,RS2,RS3, r2,v2);
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, R2[1],R2[2],R2[3],R2[4] );
        WriteLn( OutFile, V2[1],V2[2],V2[3],V2[4] );
 
      END;
 
      PROCEDURE TestANGLESGAUSS;
      VAR
         Delta1,Delta2,Delta3,Alpha1,Alpha2,Alpha3,JD1,JD2,JD3 : extended;
          RS1,RS2,RS3,r2,v2 : Vector;

      BEGIN
        ReadLn( InFile, Delta1,Delta2,Delta3,Alpha1,Alpha2,Alpha3,JD1,JD2,JD3 );
        WriteLn( OutFile,  Delta1,Delta2,Delta3,Alpha1,Alpha2,Alpha3,JD1,
            JD2,JD3 );
 
         ANGLESGAUSS ( Delta1,Delta2,Delta3,Alpha1,Alpha2,
                               Alpha3,JD1,JD2,JD3,RS1,RS2,RS3, r2,v2 );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, R2[1],R2[2],R2[3],R2[4] );
        WriteLn( OutFile, V2[1],V2[2],V2[3],V2[4] );
 
      END;
 
      PROCEDURE TestRV_RADEC;
      VAR
         Rijk,Vijk : Vector;
         rr,RtAsc,Decl,DRr,DRtAsc,DDecl,rad,radiusearthkm,vkmpersec : Extended;
         i: INTEGER;

      BEGIN
        Rad := 57.29577951308230;
        RadiusEarthkm := 6378.1363;
        VKmPerSec := 7.905366149846;
 
        ReadLn( InFile, rijk[1],rijk[2],rijk[3] );
        ReadLn( InFile, vijk[1],vijk[2],vijk[3] );
        WriteLn( OutFile, vijk[1],vijk[2],vijk[3] );

        FOR i := 1,3 DO
          BEGIN
            rijk(i) := rijk(i) / RadiusEarthKm;
            vijk(i) := vijk(i) / VKmPerSec;
          END;  { For  }

        MAG(rijk);
        MAG(vijk);
 
         RV_RADEC    ( Rijk,Vijk, Direction, rr,RtAsc,Decl,;
                               DRr,DRtAsc,DDecl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, rr,rtasc*rad,decl*rad,drr,drtasc,ddecl );
 
         RV_RADEC    ( Rijk,Vijk, FROM, rr,RtAsc,Decl,;
                               DRr,DRtAsc,DDecl );
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rijk[1],Rijk[2],Rijk[3],Rijk[4] );
        WriteLn( OutFile, Vijk[1],Vijk[2],Vijk[3],Vijk[4] );
 
      END;
 
      PROCEDURE TestRV_TRADEC;
      VAR
         Rijk[4],VIjk[4],RS[4],Rho,TRtAsc,TDecl,DRho,DTRtAsc,
               DTDecl,rad,radiusearthkm,vkmpersec : Extended;
        CHARACTER*4 Direction;
        INTEGER i;
 
        Rad := 57.29577951308230D0;
        RadiusEarthkm := 6378.1363D0;
        VKmPerSec := 7.905366149846D0;
 
      BEGIN
        ReadLn( InFile, rijk[1],rijk[2],rijk[3] );
        ReadLn( InFile, vijk[1],vijk[2],vijk[3] );
        ReadLn( InFile, rs[1],rs[2],rs[3], Direction );
        WriteLn( OutFile, rs[1],rs[2],rs[3], Direction );
        FOR i := 1,3;
            rijk(i) := rijk(i) / RadiusEarthKm;
            rs(i) := rs(i) / RadiusEarthKm;
            vijk(i) := vijk(i) / VKmPerSec;
          END;  { For  }
 
         RV_TRADEC   ( Rijk,Vijk,RS, Direction, Rho,TRtAsc,;
                               TDecl,DRho,DTRtAsc,DTDecl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rho,TRtasc*rad,TDecl*rad,DRho,DTRtasc,DTDecl );
 
        Direction := 'FROM';
         RV_TRADEC   ( Rijk,Vijk,RS, Direction, Rho,TRtAsc,;
                               TDecl,DRho,DTRtAsc,DTDecl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rijk[1],Rijk[2],Rijk[3],Rijk[4] );
        WriteLn( OutFile, Vijk[1],Vijk[2],Vijk[3],Vijk[4] );
        WriteLn( OutFile, Rs[1],Rs[2],Rs[3],Rs[4] );
 
      END;
 
      PROCEDURE TestRV_RAZEL;
      VAR
         Rijk[4],VIjk[4],RSijk[4],Lat,LST,Rho,Az,El,DRho,DAz,DEl,
               Rad : Extended;
        CHARACTER*4 Direction;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, Rijk[1],Rijk[2],Rijk[3] );
        WriteLn( OutFile, Rijk[1],Rijk[2],Rijk[3] );
        ReadLn( InFile, Vijk[1],Vijk[2],Vijk[3] );
        WriteLn( OutFile,  Vijk[1],Vijk[2],Vijk[3] );
        ReadLn( InFile, RSijk[1],RSijk[2],RSijk[3] );
        WriteLn( OutFile,  RSijk[1],RSijk[2],RSijk[3] );
        ReadLn( InFile,Lat,Lst, Direction );
        WriteLn( OutFile, Lat,Lst, Direction );
        Lat:= Lat / Rad;
        LST:= LST / Rad;
 
         RV_RAZEL    ( Rijk,Vijk,RSijk, Lat,LST, Direction,;
                               Rho,Az,El,DRho,DAz,DEl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rho,Az,El,DRho,DAz,Del );
 
        Direction := 'FROM';
         RV_RAZEL    ( Rijk,Vijk,RSijk, Lat,LST, Direction,;
                               Rho,Az,El,DRho,DAz,DEl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rijk[1],Rijk[2],Rijk[3],Rijk[4] );
        WriteLn( OutFile, Vijk[1],Vijk[2],Vijk[3],Vijk[4] );
        WriteLn( OutFile, Rsijk[1],Rsijk[2],Rsijk[3],Rsijk[4] );
 
      END;
 
      PROCEDURE TestRV_ELATLON;
      VAR
         Rijk[4], Vijk[4], rr,EclLat,EclLon,DRr,DEclLat,DEclLon,
               rad,RadiusEarthkm,VKmPerSec : Extended;
        CHARACTER*4 Direction;
        INTEGER i;
        Rad := 57.29577951308230D0;
        RadiusEarthkm := 6378.1363D0;
        VKmPerSec := 7.905366149846D0;
 
 
      BEGIN
        ReadLn( InFile, Rijk[1],Rijk[2],Rijk[3];
        WriteLn( OutFile,  Rijk[1],Rijk[2],Rijk[3] );
        ReadLn( InFile, Vijk[1],Vijk[2],Vijk[3],Direction );
        WriteLn( OutFile,  Vijk[1],Vijk[2],Vijk[3],Direction );
        FOR i := 1,3;
            rijk(i) := rijk(i) / RadiusEarthKm;
            vijk(i) := vijk(i) / VKmPerSec;
          END;  { For  }
        MAG( rijk );
        MAG( vijk );
 
         RV_ELATLON  ( Rijk,Vijk, Direction, rr,EclLat,EclLon,;
                               DRr,DEclLat,DEclLon );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, rr,EclLat*rad,EclLon*Rad,Drr,DEclLat,DEclLon;
 
        Direction := 'FROM';
         RV_ELATLON  ( Rijk,Vijk, Direction, rr,EclLat,EclLon,;
                               DRr,DEclLat,DEclLon );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rijk[1],Rijk[2],Rijk[3],Rijk[4] );
        WriteLn( OutFile, Vijk[1],Vijk[2],Vijk[3],Vijk[4] );
 
      END;
 
      PROCEDURE TestRVSEZ_RAZEL;
      VAR
        CHARACTER*4 Direction;
         RhoSez[4], DRhoSez[4],Rho,Az,El,DRho,DAz, DEl,rad : Extended;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, Rhosez[1],Rhosez[2],Rhosez[3] );
        WriteLn( OutFile,  Rhosez[1],Rhosez[2],Rhosez[3] );
        ReadLn( InFile, DRhosez[1],DRhosez[2],DRhosez[3],Direction );
        WriteLn( OutFile,  DRhosez[1],DRhosez[2],DRhosez[3],Direction );
        MAG( Rhosez );
        MAG( DRhosez );
 
         RVSEZ_RAZEL ( Rhosez,DRhosez,Direction, Rho,Az,El,;
                               DRho,DAz,DEl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rho, Az*rad, El*rad, DRho, Daz, Del );
 
        Direction := 'FROM';
         RVSEZ_RAZEL ( Rhosez,DRhosez,Direction, Rho,Az,El,;
                               DRho,DAz,DEl );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Rhosez[1],Rhosez[2],Rhosez[3] );
        WriteLn( OutFile, DRhosez[1],DRhosez[2],DRhosez[3] );
 
      END;
 
      PROCEDURE TestRADEC_ELATLON;
      VAR
         RtAsc,Decl,EclLat,EclLon,rad : Extended;
        CHARACTER*4 Direction;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, RtAsc,Decl,Direction;
        WriteLn( OutFile,  RtAsc,Decl,Direction );
        RtAsc := RtAsc / rad;
        Decl := Decl / rad;
 
         RADEC_ELATLON ( RtAsc,Decl,Direction, EclLat, EclLon);
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, EclLat*rad, EclLon*rad );
 
        Direction := 'FROM';
         RADEC_ELATLON ( RtAsc,Decl,Direction, EclLat, EclLon);
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, RtAsc*rad, Decl*rad );
 
      END;
 
      PROCEDURE TestRADEC_AZEL;
      VAR
         RtAsc,Decl,LST,Latgc,Az,El,rad : Extended;
        CHARACTER*4 Direction;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, RtAsc,Decl,LST,LatGc,Direction );
        WriteLn( OutFile,  RtAsc,Decl,LST,LatGc,Direction );
 
         RADEC_AZEL  ( RtAsc,Decl,LST,Latgc, Direction, Az,El);
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Az*rad,El*rad );
 
      END;
 
      PROCEDURE TestGIBBS;
      VAR
         R1[4], R2[4], R3[4], V2[4], Theta, Theta1, Copa,Rad : Extended;
        CHARACTER*12 Error;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, R1[1],R1[2],R1[3] );
        WriteLn( OutFile,  R1[1],R1[2],R1[3] );
        ReadLn( InFile, R2[1],R2[2],R2[3] );
        WriteLn( OutFile,  R2[1],R2[2],R2[3] );
        ReadLn( InFile, R3[1],R3[2],R3[3] );
        WriteLn( OutFile,  R3[1],R3[2],R3[3] );
        MAG( R1 );
        MAG( R2 );
        MAG( R3 );
 
         GIBBS       ( R1,R2,R3, V2, Theta,Theta1,Copa, Error);
 
        WriteLn( OutFile, '  Results: ';
        WriteLn( OutFile, V2[1],V2[2],V2[3],V2[4] );
        WriteLn( OutFile, 'angle ',theta*rad,Theta1*rad,Copa*rad,Error );
 
      END;
 
      PROCEDURE TestHERRGIBBS;
      VAR
         R1[4], R2[4], R3[4], JD1, JD2, JD3, V2[4], Theta,
               Theta1, Copa, rad,t1,t2,t3 : Extended;
        CHARACTER*12 Error;
        Rad := 57.29577951308230D0;
 
      BEGIN
        ReadLn( InFile, R1[1],R1[2],R1[3],t1 );
        JD1 := 2448608.0D0 + t1/86400.0D0;
        WriteLn( OutFile,  R1[1],R1[2],R1[3],JD1 );
        ReadLn( InFile, R2[1],R2[2],R2[3],t2;
        JD2 := 2448608.0D0 + t2/86400.0D0 );
        WriteLn( OutFile,  R2[1],R2[2],R2[3],JD2;
        ReadLn( InFile, R3[1],R3[2],R3[3],t3 );
        JD3 := 2448608.0D0 + t3/86400.0D0;
        WriteLn( OutFile,  R3[1],R3[2],R3[3],JD3 );
 
         HERRGIBBS   ( R1,R2,R3,JD1,JD2,JD3, V2, Theta,Theta1,;
                               Copa, Error );
 
        WriteLn( OutFile, '  Results: ' );
        WriteLn( OutFile, V2[1],V2[2],V2[3],V2[4] );
        WriteLn( OutFile, 'angle ',theta*rad,Theta1*rad,Copa*rad,Error );
 
      END;
 
      PROCEDURE TestLAMBERTUNIV;
      VAR
         Ro[4], r[4], DtTU, Vo[4],v[4]:vector;
        CHARACTER Dm, OverRev;
        CHARACTER*12 Error;
 
      BEGIN
        ReadLn( InFile, Ro[1],Ro[2],Ro[3] );
        WriteLn( OutFile,  Ro[1],Ro[2],Ro[3] );
        ReadLn( InFile, R[1],R[2],R[3] );
        WriteLn( OutFile,  R[1],R[2],R[3] );
        ReadLn( InFile, DtTU,dm,OverRev );
        Write(20,'(5X,A1,A1,F12.8)') dm,OverRev,DtTU );
 
        MAG( Ro );
        MAG( R );
        WriteLn( OutFile,  dm,' : ',overrev,' ',dttu;
 
         LAMBERTUNIV ( ro,r, dm,OverRev, DtTU, vo,v, Error );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Vo[1],Vo[2],Vo[3],Vo[4] );
        WriteLn( OutFile, V[1],V[2],V[3],V[4] );
 
      END;
 
      PROCEDURE TestLAMBERTBATTIN;
      VAR
         Ro[4], r[4], DtTU, Vo[4],v[4]:vector;
        CHARACTER Dm, OverRev;
        CHARACTER*12 Error;
 
      BEGIN
        ReadLn( InFile, Ro[1],Ro[2],Ro[3] );
        WriteLn( OutFile,  Ro[1],Ro[2],Ro[3] );
        ReadLn( InFile, R[1],R[2],R[3] );
        WriteLn( OutFile,  R[1],R[2],R[3] );
        ReadLn( InFile, DtTU, dm,OverRev );
        WriteLn( OutFile,  dm,OverRev,DtTU );
 
        MAG( Ro );
        MAG( R );
 
         LAMBERTBATTIN ( ro,r, dm,OverRev, DtTU, vo,v, Error );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, Vo[1],Vo[2],Vo[3],Vo[4] );
        WriteLn( OutFile, V[1],V[2],V[3],V[4] );
 
      END;
 
      PROCEDURE TestTARGET;
      VAR
         RInt[4],VInt[4],RTgt[4],VTgt[4],DtTU,V1t[4],V2t[4],
               DV1[4],DV2[4] : Extended;
        CHARACTER Kind, Dm;
        CHARACTER*12 Error;
 
      BEGIN
        ReadLn( InFile, );
        WriteLn( OutFile, 'In ' );
 
         TARGET      ( RInt,VInt,RTgt,VTgt, Dm,Kind, DtTU,;
                               V1t,V2t,DV1,DV2, Error  );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, );
 
      END;
 
   ! --------------------  REDUC's -----------------------------------;
 
      PROCEDURE TestInitNutation;
      VAR
        CHARACTER*64 FileN1,FileN2;
        INTEGER IAr(5,106);
          RAr(4,106);
        INTEGER iIAr(5,4);
          iRAr(4,4);
        INTEGER pIAr(10,85);
          pRAr(4,85);
 
      BEGIN
        ReadLn( InFile, FileN1 );
        ReadLn( InFile, FileN2 );
        WriteLn( OutFile, 'In ' );
 
        InitNutation ( FileN1,FileN2,IAr,RAr,iIAr,iRAr,pIAr,pRAr );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, IAr(1,1),' ',RAr(1,1)*3600.0D0/0.0001D0 );
        WriteLn( OutFile, IAr(2,1),' ',RAr(2,1)*3600.0D0/0.0001D0 );
        WriteLn( OutFile, IAr(3,1),' ',RAr(3,1)*3600.0D0/0.0001D0 );
      END;
 
      PROCEDURE TestConvTime;
      VAR
        INTEGER Year, Mon, Day, Hr, MIN;
        INTEGER TimeZone;
        CHARACTER TypeUTIn;
        CHARACTER*64 FileN1;
        CHARACTER*12  Error;
         DUT1, DAT, xp, yp, UT1, TUT1, JDUT1, UTC, TAI, TDT, TTDT,
        Sec, JDTDT, TDB, TTDB, JDTDB : Extended;
 
      BEGIN
        ReadLn( InFile, FileN1 );
        ReadLn( InFile, Year,Mon,Day,Hr,Min,Sec,TimeZone,TypeUtIn );
        WriteLn( OutFile, 'In',Year,Mon,Day,Hr,Min,Sec,TimeZone,TypeUtIn );
 
        CONVTIME ( FileN1,Year, Mon, Day, Hr, MIN, SEC,;
                           TimeZone, TypeUTIn, DUT1, DAT, xp, yp, UT1,;
                           TUT1, JDUT1, UTC, TAI, TDT, TTDT, JDTDT,TDB,;
                           TTDB, JDTDB, Error );
 
        WriteLn( OutFile, '  Results:' );
        WriteLn( OutFile, DUT1,' ',DAT,' ',xp,' ',yp,' ' );
        WriteLn( OutFile, UT1,' ',TUT1,' ',JDUT1,' ',UTC,' ',TAI,' ',tdt );
        WriteLn( OutFile, TTDT,' ',JDTDT,' ',TDB,' ',TTDB,' ',JDTDB );
      END;
 
      PROCEDURE TestPrecession;
      VAR
        CHARACTER*4 Direction;
        INTEGER i;
         rJ2000[4], vJ2000[4], rMOD[4], vMOD[4], TTDB:vector;
 
      BEGIN
        ReadLn( InFile, rJ2000[1],rJ2000[2],rJ2000[3] );
        ReadLn( InFile, vJ2000[1],vJ2000[2],vJ2000[3],TTDB,Direction );
        MAG( rJ2000 );
        MAG( vJ2000 );
        WriteLn( OutFile, 'In r ',rJ2000[1],rJ2000[2],rJ2000[3] );
        WriteLn( OutFile, '   v ',vJ2000[1],vJ2000[2],vJ2000[3], );
                    ':',Direction,':',ttdb );
        FOR i := 1, 4;
            rJ2000(i) := rJ2000(i)/6378.1363D0;
            vJ2000(i) := vJ2000(i)/7.905366149846D0;
          END;  { For  }
 
        PRECESSION ( rJ2000, vJ2000, Direction, rMOD,;
                              vMOD,TTDB );
 
        FOR i := 1, 4;
            rMOD(i) := rMOD(i)*6378.1363D0;
            vMOD(i) := vMOD(i)*7.905366149846D0;
          END;  { For  }
        WriteLn( OutFile, 'Results:r ',rMOD[1],rMOD[2],rMOD[3] );
        WriteLn( OutFile, '        v ',vMOD[1],vMOD[2],vMOD[3] );
 
      END;
 
      PROCEDURE TestNutation;
      VAR
        CHARACTER*4 Direction;
         rMOD[4], vMOD[4], rTOD[4], vTOD[4],rad,;
               DeltaPsi, TrueEps, TTdb, RAr(4,106),iRAr(4,4),pRAr(4,85) : Extended;
        INTEGER IAr(5,106), iIAr(5,4), pIAr(10,85);
        CHARACTER*64 FileN1,FileN2;
 
        Rad := 57.29577951308230D0;
      BEGIN
        ReadLn( InFile, FileN1 );
        ReadLn( InFile, FileN2 );
        InitNutation ( FileN1,FileN2,IAr,RAr,iIAr,iRAr,pIAr,pRAr );
 
        ReadLn( InFile, rMOD[1],rMOD[2],rMOD[3] );
        ReadLn( InFile, vMOD[1],vMOD[2],vMOD[3],DeltaPsi,TrueEps,TTDB, );
                   Direction );
        DeltaPsi := DeltaPsi / Rad;
        TrueEps := TrueEps / Rad;
        MAG( rMOD );
        MAG( vMOD );
        WriteLn( OutFile, 'In r ',rMOD[1],rMOD[2],rMOD[3] );
        WriteLn( OutFile, '   v ',vMOD[1],vMOD[2],vMOD[3],Direction );
 
        NUTATION ( rMOD, vMOD, Direction, rTOD, vTOD,;
                            DeltaPsi,TrueEps, TTdb, IAr,RAr, iIAr,iRAr,;
                            pIAr,pRAr );
 
        WriteLn( OutFile, 'Results:r ',rTOD[1],rTOD[2],rTOD[3] );
        WriteLn( OutFile, '        v ',vTOD[1],vTOD[2],vTOD[3] );
      END;
 
      PROCEDURE TestSidereal;
      VAR
        INTEGER i;
        CHARACTER*4 Direction;
         rTOD[4], vTOD[4], rECEF[4], vECEF[4], JDUT1, DeltaPsi,
               TrueEps,Rad : Extended;
 
        Rad := 57.29577951308230D0;
      BEGIN
        ReadLn( InFile, rTOD[1],rTOD[2],rTOD[3] );
        ReadLn( InFile, vTOD[1],vTOD[2],vTOD[3], JDUT1, );
                   DeltaPsi, TrueEps,Direction );
 
        MAG( rTOD );
        MAG( vTOD );
        DeltaPsi := DeltaPsi / Rad;
        TrueEps := TrueEps / Rad;
        WriteLn( OutFile, 'In r ',rTOD[1],rTOD[2],rTOD[3] );
        WriteLn( OutFile, '   v ',vTOD[1],vTOD[2],vTOD[3],Direction );
        FOR i := 1, 4;
            rTOD(i) := rTOD(i)/6378.1363D0;
            vTOD(i) := vTOD(i)/7.905366149846D0;
          END;  { For  }
 
        SIDEREAL ( rTOD, vTOD, Direction, rECEF, vECEF,;
                            JDUT1,DeltaPsi, TrueEps);
 
        FOR i := 1, 4;
            rECEF(i) := rECEF(i)*6378.1363D0;
            vECEF(i) := vECEF(i)*7.905366149846D0;
          END;  { For  }
        WriteLn( OutFile, 'Results:r ',rECEF[1],rECEF[2],rECEF[3] );
        WriteLn( OutFile, '        v ',vECEF[1],vECEF[2],vECEF[3] );
      END;
 
      PROCEDURE TestPolarM;
      VAR
        CHARACTER*4 Direction;
         rECEF[4], vECEF[4], rECEFwpm[4], vECEFwpm[4], xp, yp,rad:vector;
 
        Rad := 57.29577951308230D0;
        ReadLn( InFile, rECEF[1],rECEF[2],rECEF[3] );
        ReadLn( InFile, vECEF[1],vECEF[2],vECEF[3],xp,yp,Direction );
        MAG( rECEF );
        MAG( vECEF );
        xp := xp / (3600.0D0*Rad);
        yp := yp / (3600.0D0*Rad);
        WriteLn( OutFile, 'In r ',rECEF[1],rECEF[2],rECEF[3] );
        WriteLn( OutFile, '   v ',vECEF[1],vECEF[2],vECEF[3],Direction );
 
        POLARM ( rECEF, vECEF, Direction, rECEFwpm,;
                          vECEFwpm,xp, yp);
 
        WriteLn( OutFile, 'Results:r ',rECEFwpm[1],rECEFwpm[2],rECEFwpm[3] );
        WriteLn( OutFile, '        v ',vECEFwpm[1],vECEFwpm[2],vECEFwpm[3] );
      END;
 
      PROCEDURE TestTrueMean;
      VAR
        CHARACTER*4 Direction;
        CHARACTER*64 FileN1,FileN2;
         rMOD[4], vMOD[4], rTM[4], vTM[4], DeltaPsi,;
               TrueEps, TTdb,Rad, RAr(4,106),iRAr(4,4),pRAr(4,85);
        INTEGER IAr(5,106), iIAr(5,4), pIAr(10,85);
 
        Rad := 57.29577951308230D0;
      BEGIN
        ReadLn( InFile, FileN1 );
        ReadLn( InFile, FileN2 );
        InitNutation ( FileN1,FileN2,IAr,RAr,iIAr,iRAr,pIAr,pRAr );
 
        ReadLn( InFile, rMOD[1],rMOD[2],rMOD[3] );
        ReadLn( InFile, vMOD[1],vMOD[2],vMOD[3],DeltaPsi,;
                    TrueEps,Ttdb,Direction );
        MAG( rMOD );
        MAG( vMOD );
        WriteLn( OutFile, 'In r ',rMOD[1],rMOD[2],rMOD[3] );
        WriteLn( OutFile, '   v ',vMOD[1],vMOD[2],vMOD[3],' ',DeltaPsi,
                    TrueEps,Ttdb,Direction );
        DeltaPsi := DeltaPsi / Rad;
        TrueEps := TrueEps / Rad;
 
        TrueMean ( rMOD,vMOD, Direction, rTM,vTM,
                             DeltaPsi,TrueEps, TTdb, IAr, RAr );
 
        WriteLn( OutFile, 'Results:r ',rTM[1],rTM[2],rTM[3] );
        WriteLn( OutFile, '        v ',vTM[1],vTM[2],vTM[3] );
      END;
 
      PROCEDURE TestFK4;
      VAR
        CHARACTER*4 Direction;
         rJ2000[4], vJ2000[4], rFK4[4], vFK4[4]:vector;
 
      BEGIN
        ReadLn( InFile, rJ2000[1],rJ2000[2],rJ2000[3] );
        ReadLn( InFile, vJ2000[1],vJ2000[2],vJ2000[3],Direction );
        MAG( rJ2000 );
        MAG( vJ2000 );
        WriteLn( OutFile, 'In r ',rJ2000[1],rJ2000[2],rJ2000[3] );
        WriteLn( OutFile, '   v ',vJ2000[1],vJ2000[2],vJ2000[3],Direction );
 
        FK4 ( rJ2000, vJ2000, Direction, rFK4, vFK4 );
 
        WriteLn( OutFile, 'Results: ' );
        WriteLn( OutFile, '        r ',rFK4[1],rFK4[2],rFK4[3] );
        WriteLn( OutFile, '        v ',vFK4[1],vFK4[2],vFK4[3] );
      END;





BEGIN { --------------------------- M A I N ----------------------------- }

        IFileName := 'testastf.dat'
        ASSIGN( InFile,IFileName );
        RESET( InFile );

        ASSIGN( OutFile,'testastf.out' );
        REWriteLn( OutFile );

        i := 1
        WHILE (i >= 0) FOR
          BEGIN
            ! -------------------- Tests for Math ----------------------
            ReadLn( 10,'(I4,4X,A15)', END:=999 ) NumChose,Title
            IF (NumChose = 0 ) THEN
                ReadLn(10,'(I4,4X,A15)',END:=999 ) NumChose,Title
              END;
            Write(*,*) ' ------ xx',Title,' Test Case --'
            WriteLn( OutFile, '-- Test Case --','------ xx',NumChose,Title,
                                 '-----------------------'

            IF (NumChose = 2 ) THEN
                TestGETPART;
              END;
            IF (NumChose = 3 ) THEN
                TestGETPARTL;
              END;
            IF (NumChose = 4 ) THEN
                TestGetPartR;
              END;
            IF (NumChose = 5 ) THEN
                TestFACTORIAL;
              END;
            IF (NumChose = 6 ) THEN
                TestBINOMIAL;
              END;
            IF (NumChose = 7 ) THEN
                TestMIN;
              END;
            IF (NumChose = 8 ) THEN
                TestMAX;
              END;
            IF (NumChose = 9 ) THEN
                TestPLANE;
              END;
            IF (NumChose = 10 ) THEN
                TestACOSH;
              END;
            IF (NumChose = 11 ) THEN
                TestSINH;
              END;
            IF (NumChose = 12 ) THEN
                TestASINH;
              END;
            IF (NumChose = 13 ) THEN
                TestATANH;
              END;
            IF (NumChose = 14 ) THEN
                TestDOT;
              END;
            IF (NumChose = 15 ) THEN
                TestCROSS;
              END;
            IF (NumChose = 16 ) THEN
                TestMAG;
              END;
            IF (NumChose = 17 ) THEN
                TestNORM;
              END;
            IF (NumChose = 18 ) THEN
                TestROT1;
              END;
            IF (NumChose = 19 ) THEN
                TestROT2;
              END;
            IF (NumChose = 20 ) THEN
                TestROT3;
              END;
            IF (NumChose = 21 ) THEN
                TestADDVEC;
              END;
            IF (NumChose = 22 ) THEN
                TestADD3VEC;
              END;
            IF (NumChose = 23 ) THEN
                TestLNCOM1;
              END;
            IF (NumChose = 24 ) THEN
                TestLNCOM2;
              END;
            IF (NumChose = 25 ) THEN
                TestLNCOM3;
              END;
            IF (NumChose = 26 ) THEN
                TestPOLYFIT;
              END;
            IF (NumChose = 27 ) THEN
                TestANGLE;
              END;
            IF (NumChose = 29 ) THEN
                TestFACTOR;
              END;
            IF (NumChose = 30 ) THEN
                TestQUADRATIC;
              END;
            IF (NumChose = 31 ) THEN
                TestCUBIC;
              END;
            IF (NumChose = 32 ) THEN
                TestQUARTIC;
              END;
            IF (NumChose = 33 ) THEN
                TestMATSCALE;
              END;
            IF (NumChose = 34 ) THEN
                TestMATMULT;
              END;
            IF (NumChose = 35 ) THEN
                TestMATADD;
              END;
            IF (NumChose = 36 ) THEN
                TestMATSUB;
              END;
            IF (NumChose = 37 ) THEN
                TestMATTRANS;
              END;
            IF (NumChose = 38 ) THEN
                TestMAKEMAT;
              END;
            IF (NumChose = 41 ) THEN
                TestMATINVERSE;
              END;
            IF (NumChose = 42 ) THEN
                TestPRINTMAT;
              END;
            IF (NumChose = 43 ) THEN
                TestFILEPRINTMAT;
              END;
            IF (NumChose = 44 ) THEN
                TestFILEEXPPRINTMAT;
              END;
            IF (NumChose = 45 ) THEN
                TestDETERMINANT;
              END;

            { -------------------- Tests for Time ---------------------- }
            IF (NumChose = 100 ) THEN
                TestUpCaseSt;
              END;
            IF (NumChose = 101 ) THEN
                TestINITTIME;
              END;
            IF (NumChose = 102 ) THEN
                TestGETINTMON;
              END;
            IF (NumChose = 103 ) THEN
                TestGETINTDAY;
              END;
            IF (NumChose = 104 ) THEN
                TestDAYOFWEEK;
              END;
            IF (NumChose = 105 ) THEN
                TestDAYLIGHTST;
              END;
            IF (NumChose = 106 ) THEN
                TestJULIANDAY;
              END;
            IF (NumChose = 107 ) THEN
                TestJULIANDAYALL;
              END;
            IF (NumChose = 108 ) THEN
                TestDAYS2MDHMS;
              END;
            IF (NumChose = 109 ) THEN
                TestINVJULIANDAY;
              END;
            IF (NumChose = 110 ) THEN
                TestFINDDAYS;
              END;
            IF (NumChose = 111 ) THEN
                TestLSTIME;
              END;
            IF (NumChose = 112 ) THEN
                TestSUNRISESET;
              END;
            IF (NumChose = 113 ) THEN
                TestMOONRISESET;
              END;
            IF (NumChose = 114 ) THEN
                TestHMS_SEC;
              END;
            IF (NumChose = 115 ) THEN
                TestHMS_UT;
              END;
            IF (NumChose = 116 ) THEN
                TestHMS_RAD;
              END;
            IF (NumChose = 117 ) THEN
                TestDMS_RAD;
              END;
            { -------------------- Tests for 2body --------------------- }
            IF (NumChose = 150 ) THEN
                TestELORB;
              END;
            IF (NumChose = 151 ) THEN
                TestRANDV;
              END;
            IF (NumChose = 152 ) THEN
                TestFINDC2C3;
              END;
            IF (NumChose = 153 ) THEN
                TestNEWTONE;
              END;
            IF (NumChose = 154 ) THEN
                TestNEWTONM;
              END;
            IF (NumChose = 155 ) THEN
                TestNEWTONNU;
              END;
            IF (NumChose = 156 ) THEN
                TestKEPLER;
              END;
            IF (NumChose = 157 ) THEN
                TestFINDTOF;
              END;
            IF (NumChose = 158 ) THEN
                TestIJKTOLATLONA;
              END;
            IF (NumChose = 159 ) THEN
                TestGEOCGEOD;
              END;
            IF (NumChose = 160 ) THEN
                TestSIGHT;
              END;
            IF (NumChose = 161 ) THEN
                TestSUN;
              END;
            IF (NumChose = 162 ) THEN
                TestMOON;
              END;
            IF (NumChose = 163 ) THEN
                TestLIGHT;
              END;
            IF (NumChose = 164 ) THEN
                TestCHECKHITEARTH;
              END;
            IF (NumChose = 165 ) THEN
                TestSATFOV;
              END;
            IF (NumChose = 166 ) THEN
                TestRNGAZ;
              END;
            IF (NumChose = 167 ) THEN
                TestPATH;
              END;

            { -------------------- Tests for iod ----------------------- }
            IF (NumChose = 200 ) THEN
                TestSITE;
              END;
            IF (NumChose = 201 ) THEN
                TestANGLESLAPLACE;
              END;
            IF (NumChose = 202 ) THEN
                TestANGLESGAUSS;
              END;
            IF (NumChose = 203 ) THEN
                TestRV_RADEC;
              END;
            IF (NumChose = 204 ) THEN
                TestRV_TRADEC;
              END;
            IF (NumChose = 205 ) THEN
                TestRV_RAZEL;
              END;
            IF (NumChose = 206 ) THEN
                TestRV_ELATLON;
              END;
            IF (NumChose = 207 ) THEN
                TestRVSEZ_RAZEL;
              END;
            IF (NumChose = 208 ) THEN
                TestRADEC_ELATLON;
              END;
            IF (NumChose = 209 ) THEN
                TestRADEC_AZEL;
              END;
            IF (NumChose = 210 ) THEN
                TestGIBBS;
              END;
            IF (NumChose = 211 ) THEN
                TestHERRGIBBS;
              END;
            IF (NumChose = 212 ) THEN
                TestLAMBERTUNIV;
              END;
            IF (NumChose = 213 ) THEN
                TestLAMBERTBATTIN;
              END;
            IF (NumChose = 214 ) THEN
                TestTARGET;
              END;

            { -------------------- Tests for reduc --------------------- }
            IF (NumChose = 300 ) THEN
                TestInitNutation;
              END;
            IF (NumChose = 301 ) THEN
                TestCONVTIME;
              END;
            IF (NumChose = 302 ) THEN
                TestPRECESSION;
              END;
            IF (NumChose = 303 ) THEN
                TestNUTATION;
              END;
            IF (NumChose = 304 ) THEN
                TestSIDEREAL;
              END;
            IF (NumChose = 305 ) THEN
                TestPOLARM;
              END;
            IF (NumChose = 306 ) THEN
                TestTrueMean;
              END;
            IF (NumChose = 307 ) THEN
                TestFK4;
              END;

           END; { While }

        CLOSE( InFile );
        CLOSE( OutFile );

    END. { Program Testastf }

