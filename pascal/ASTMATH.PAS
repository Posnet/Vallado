(*     ----------------------------------------------------------------      *)

                               UNIT ASTMATH;
(*
*    this file contains miscellaneous math functions.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2007
*                              by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*              30 may 07  david vallado
*                           3rd edition baseline
*    changes :
*              21 jul 05  david vallado
*                           2nd printing baseline
*              28 Jan 04  David Vallado                                       
*                           Update headers                                    
*              14 May 01  David Vallado                                       
*                           2nd edition baseline                              
*              23 Nov 87  David Vallado                                       
*                           Original baseline                                 
*                                                                           
*      ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    TYPE
        Vector     = ARRAY[1..4] of EXTENDED;
        Array16    = ARRAY[1..16] of EXTENDED;
        Array15by2 = ARRAY[1..15,1..2] of EXTENDED;
        Str250     = STRING[250];
        Str64      = STRING[64];

        Intarray   = ARRAY[1..6] of INTEGER;

        MatrixDataPtr = ^MatrixData;
        MatrixData    = RECORD
                          Index     : INTEGER;
                          Number    : EXTENDED;
                          Next,Last : MatrixDataPtr;
                        END;
        MatrixPtr     = RECORD
                          NumRows,NumCols : INTEGER;
                          DPtr            : MatrixDataPtr;
                          Head,Tail       : MatrixDataPtr;
                        END;
        Matrix        = ^MatrixPtr;

     { -------------------- Misc functions for PASCAL -------------- }

FUNCTION  FACTORIAL          ( x                                     : INTEGER  ) : EXTENDED;

FUNCTION  BINOMIAL           ( i,j                                   : INTEGER  ) : EXTENDED;

FUNCTION  SGN                ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  MODFUNC            ( XVal, Modby                           : EXTENDED ) : EXTENDED;

FUNCTION  POWER              ( Base, Exponent                        : EXTENDED ) : EXTENDED;

FUNCTION  LOG                ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  MIN                ( X, Y                                  : EXTENDED ) : EXTENDED;

FUNCTION  MAX                ( X, Y                                  : EXTENDED ) : EXTENDED;

PROCEDURE PLANE              ( x1,y1,z1,x2,y2,z2,x3,y3,z3            : EXTENDED;
                               VAR a,b,c,d                           : EXTENDED );
{}
     { -------------------  Trigonometric Functions  --------------- }

FUNCTION  TAN                ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  COT                ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  CSC                ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  SEC                ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  ATAN2              ( SinValue, CosValue                    : EXTENDED ) : EXTENDED;

FUNCTION  ASIN               ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  ACOS               ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  COSH               ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  ACOSH              ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  SINH               ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  ASINH              ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  TANH               ( XVal                                  : EXTENDED ) : EXTENDED;

FUNCTION  ATANH              ( XVal                                  : EXTENDED ) : EXTENDED;

   { ------------------------ Vector Operations ----------------- }

FUNCTION  DOT                ( Vec1,Vec2                             : Vector   ) : EXTENDED;

PROCEDURE CROSS              ( Vec1,Vec2                             : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE MAG                ( VAR Vec                               : Vector   );

PROCEDURE NORM               ( Vec                                   : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE ROT1               ( Vec                                   : Vector;
                               XVal                                  : EXTENDED;
                               VAR OutVec                            : Vector   );

PROCEDURE ROT2               ( Vec                                   : Vector;
                               XVal                                  : EXTENDED;
                               VAR OutVec                            : Vector   );

PROCEDURE ROT3               ( Vec                                   : Vector;
                               XVal                                  : EXTENDED;
                               VAR OutVec                            : Vector   );

PROCEDURE ADDVEC             ( Vec1,Vec2                             : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE ADD3VEC            ( Vec1,Vec2,Vec3                        : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE LNCOM1             ( A                                     : EXTENDED;
                               Vec                                   : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE LNCOM2             ( A1,A2                                 : EXTENDED;
                               Vec1,Vec2                             : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE LNCOM3             ( A1,A2,A3                              : EXTENDED;
                               Vec1,Vec2,Vec3                        : Vector;
                               VAR OutVec                            : Vector   );

PROCEDURE ANGLE              ( Vec1,Vec2                             : Vector;
                               VAR Theta                             : EXTENDED );

     { ---------------------- Polynomial routines ----------------- }

PROCEDURE POLYFIT            ( Degree,NumPts                         : INTEGER;
                               DataPoints                            : Matrix;
                               VAR Coeff                             : Matrix;
                               VAR MinX,MinY                         : EXTENDED );

PROCEDURE FACTOR             ( Poly                                  : Array16;
                               NRoots                                : INTEGER;
                               VAR Roots                             : Array15by2 );

PROCEDURE QUADRATIC          ( a,b,c                                 : EXTENDED;
                               VAR R1r,R1i,R2r,R2i                   : EXTENDED );

PROCEDURE CUBIC              ( a,b,c,d                               : EXTENDED;
                               VAR R1r,R1i,R2r,R2i,R3r,R3i           : EXTENDED );

PROCEDURE QUARTIC            ( a,b,c,d,e                             : EXTENDED;
                               VAR R1r,R1i,R2r,R2i,R3r,R3i,R4r,R4i   : EXTENDED );
{}
   { ------------------------- Matrix Operations ----------------- }

PROCEDURE INITMATRIX         ( Rows,Cols, SRow,SCol                  : INTEGER;
                               VAR A                                 : Matrix   );

PROCEDURE DELMATRIX          ( VAR A                                 : Matrix   );

FUNCTION  GETVAL             ( VAR A                                 : Matrix;
                               Row,Col                               : INTEGER  ): EXTENDED;

PROCEDURE ASSIGNVAL          ( VAR A                                 : Matrix;
                               Row,Col                               : INTEGER;
                               Number                                : EXTENDED );

PROCEDURE MATSCALE           ( Mat1                                  : Matrix;
                               Scale                                 : EXTENDED;
                               VAR Mat2                              : Matrix   );

PROCEDURE MATMULT            ( Mat1,Mat2                             : Matrix;
                               Mat1r,Mat1c,Mat2c                     : INTEGER ;
                               VAR Mat3                              : Matrix   );

PROCEDURE MATADD             ( Mat1,Mat2                             : Matrix;
                               Mat1r,Mat1c                           : INTEGER ;
                               VAR Mat3                              : Matrix   );

PROCEDURE MATSUB             ( Mat1,Mat2                             : Matrix;
                               Mat1r,Mat1c                           : INTEGER ;
                               VAR Mat3                              : Matrix   );

PROCEDURE MATTRANS           ( Mat1                                  : Matrix;
                               Mat1r,Mat1c                           : INTEGER ;
                               VAR Mat2                              : Matrix   );

PROCEDURE MAKEMAT            ( Angl                                  : EXTENDED;
                               Numbr                                 : INTEGER;
                               VAR Matr                              : Matrix   );

PROCEDURE MatVecMult         ( Mat                                   : Matrix;
                               Vec                                   : Vector;
                               Mat1r,Mat1c                           : Integer ;
                               VAR VecOut                            : Vector );

PROCEDURE MATINVERSE         ( Mat                                   : Matrix;
                               Order                                 : INTEGER;
                               VAR MatInv                            : Matrix   );


PROCEDURE PRINTMAT           ( Mat1                                  : Matrix;
                               DecNum                                : INTEGER;
                               Title                                 : Str64    );

FUNCTION  Determinant        ( Mat1                                  : Matrix;
                               Order                                 : INTEGER  ) : EXTENDED;

PROCEDURE FILEPRINTMAT       ( Mat1                                  : Matrix;
                               Title                                 : STR64;
                               Conv                                  : EXTENDED;
                               NumRows                               : INTEGER;
                               VAR OutFile                           : TEXT     );

PROCEDURE FILEEXPPRINTMAT    ( Mat1                                  : Matrix;
                               Title                                 : STR64;
                               Conv                                  : EXTENDED;
                               NumRows                               : INTEGER;
                               VAR OutFile                           : TEXT     );

(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION FACTORIAL
|
|  This FUNCTION finds the value of a FACTORIAL.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    x           - Input value
|
|  Outputs       :
|    FUNCTION    - answer
|
|  Locals        :
|    Temp        - Temporary variable
|    i           - Index
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION  FACTORIAL          ( x                                     : INTEGER ) : EXTENDED;
   VAR
      Temp : EXTENDED;
      i    : INTEGER;
    BEGIN
      Temp:= 1.0;
      FOR i:= 2 to x DO
          Temp:= Temp * i;
      FACTORIAL:= Temp;
    END;  { FUNCTION FACTORIAL }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION BINOMIAL
|
|  This FUNCTION finds the value of a BINOMIAL coefficient
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    i           -
|    j           -
|
|  Outputs       :
|    FUNCTION    - answer
|
|  Locals        :
|    None.
|
|  Coupling      :
|    FACTORIAL   - Finds the FACTORIAL of a number
|
 ----------------------------------------------------------------------------- }

FUNCTION  BINOMIAL           ( i,j                                   : INTEGER ) : EXTENDED;
    BEGIN
      BINOMIAL:= FACTORIAL(j) / ( FACTORIAL(i)*FACTORIAL(j-i) );
    END;  { FUNCTION BINOMIAL }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION SGN
|
|  This FUNCTION determines the sign of a number.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - Value to determine sign of
|
|  OutPuts       :
|    SGN         - Result                         +1 or -1
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION SGN                   ( XVal                      : EXTENDED ) : EXTENDED;
   VAR
     Temp : EXTENDED;
   BEGIN
     IF XVal > 0.0 THEN
         Temp:= 1.0
       ELSE
         Temp:= -1.0;
     SGN:= Temp;
   END;  { FUNCTION SGN }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION MODFUNC
|
|  This FUNCTION performs the MOD operation for REALs.
|
|  Algorithm     : Assign a temporary variable
|                  Subtract off an INTEGER number of values while the xval is
|                     too large
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - Value to MOD
|    ModBy       - Value to MOD with
|
|  OutPuts       :
|    MODFUNC     - Result                         -ModBy <=  Answer  <= +ModBy
|
|  Locals        :
|    TempValue   - Temporary EXTENDED value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION MODFUNC             ( XVal,Modby                            : EXTENDED ) : EXTENDED;
   VAR
       TempValue: EXTENDED;
   BEGIN
     TempValue := XVal;
     WHILE ABS(TempValue) > ModBy DO
         TempValue:= TempValue - INT(XVal/ModBy)*ModBy;
     MODFUNC:= TempValue;
   END;  { FUNCTION MODFUNC }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION POWER
|
|  This FUNCTION performs the raising of a base to a POWER.  Notice the many
|    statements to allow processing of negative INTEGER values.
|
|  Algorithm     : IF the base and exponent are positive, calculate the answer
|                  Otherwise, check for 0 base 1st, THEN 0 exponent
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Base        - Base value
|    Pwr         - POWER to raise base to
|
|  OutPuts       :
|    POWER       - Result
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
|  References    : PC Magazine, 10 Sep 91, pg. 465-466.
|
 ----------------------------------------------------------------------------- }

FUNCTION POWER               ( Base, Exponent                        : EXTENDED ) : EXTENDED;
   CONST
     Small     : EXTENDED = 0.00000000001;
   VAR Recip   : EXTENDED;

   FUNCTION IsInt( Num : EXTENDED ): Boolean;
     BEGIN
       IsInt:= ((ABS(FRAC(Num)) < Small) or
               (1.0-ABS(FRAC(Num)) < Small)) and (ABS(Num) < MAXLongInt );
     END;  { Internal FUNCTION IsInt }

   FUNCTION SignFix( Num : LongInt ): ShortInt;
     BEGIN
       IF ODD(Num) THEN
           SignFix:= -1
         ELSE
           SignFix:= 1;
     END;  { Internal FUNCTION SignFix }

   BEGIN
     IF ABS(Base) > Small THEN
       BEGIN
         IF ABS(Exponent) > Small THEN
           BEGIN
             { --------------- Base is (+) and Exponent <> 0 ---------------- }
             IF Base > 0.0 THEN
                 POWER := Exp( Exponent * LN( Base ) )
               ELSE
               BEGIN
                 { --------- Base is (-) and Exponent is an INTEGER --------- }
                 IF IsInt( Exponent ) THEN
                     POWER := Exp( Exponent * LN(-Base) )*SignFix( ROUND(Exponent) )
                   ELSE
                   { ----------- Base is (-) and Exponent is real ----------- }
                   BEGIN
                     Recip:= 1.0/Exponent;
                     { ---- Base is (-) and Exponent is recip of odd int ---- }
                     IF ( ABS(ROUND(Recip) - Recip) < 0.000001 ) and
                        ( ODD(ROUND(Recip)) ) THEN
                         POWER := -Exp( Exponent * LN(-Base) )
                       ELSE
                       BEGIN
                         POWER:= 0.0;
                         Writeln( 'Error in POWER with base = ',base,' and Pwr = ',Exponent );
                       END;
                   END;
               END;
           END
           ELSE
             { ---- IF Exponent = 0 ---- }
             POWER:= 1.0;
       END
       ELSE
         { -------- IF Base = 0 -------- }
         POWER:= 0.0;
   END;  { FUNCTION POWER }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION LOG
|
|  This FUNCTION performs the LOG base 10 problem.
|
|  Algorithm     : IF the x is positive, calculate the answer
|                  Otherwise, set the answer to 0.0
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    X           - Value to take the LOG base 10 of
|
|  OutPuts       :
|    LOG         - Result
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION LOG                 ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
      Lfac       : EXTENDED = 0.4342944819;      {1.0/LN(10)}
   BEGIN
      IF XVal > 0.0 THEN
          LOG := Lfac*LN(XVal)
        ELSE
          LOG := -1.0e37;
   END;   { FUNCTION LOG }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION MIN
|
|  This FUNCTION determines the minimum of 2 values.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    X           - Value number 1
|    Y           - Value number 2
|
|  OutPuts       :
|    MIN         - Minimum of x or y
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION MIN                 ( X, Y                                  : EXTENDED ) : EXTENDED;
    BEGIN
      IF X < Y THEN
          MIN := X
        ELSE
          MIN := Y;
    END;  { FUNCTION MIN }



{ ------------------------------------------------------------------------------
|
|                           FUNCTION MAX
|
|  This FUNCTION determines the maximum of 2 values.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    X           - Value number 1
|    Y           - Value number 2
|
|  OutPuts       :
|    MAX         - Minimum of x or y
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION MAX                 ( X, Y                                  : EXTENDED ) : EXTENDED;
    BEGIN
      IF X > Y THEN
          MAX := X
        ELSE
          MAX := Y;
    END;  { FUNCTION MAX }

{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PLANE
|
| This PROCEDURE calculates the equation of a PLANE given 3 points
|   pt1 - x1,y1,z1, pt2 - x2,y2,z2, pt3 - x3,y3,z3 , and outputs the
|   a b c d variables describing the PLANE. NOTE that the general equation
|   of a PLANE is defined here as:  ax + by + cz + d = 0  and the values
|   are obtained by solving the ordered determinant  x  y  z   1
|                                                    x1 y1 z1  1   =0
|                                                    x2 y2 z2  1
|                                                    x3 y3 z3  1
|
|  Algorithm     : find the line differences for each set of points
|                  Calculate the coefficients of the PLANE
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    x1,y1,z1    - point # 1
|    x2,y2,z2    - point # 2
|    x3,y3,z3    - point # 3
|
|  OutPuts       :
|    a,b,c,d     - constants for the equation of the PLANE
|
|  Locals        :
|    z23
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE PLANE              ( x1,y1,z1,x2,y2,z2,x3,y3,z3            : EXTENDED;
                               VAR a,b,c,d                           : EXTENDED);
   VAR
     z23,y23,x23,yz23,yz32,xz23,xz32,xy23,xy32 : EXTENDED;
   BEGIN
     z23:= z2-z3;
     y23:= y2-y3;
     x23:= x2-x3;

     yz23:= y2*z3;
     yz32:= y3*z2;
     xz23:= x2*z3;
     xz32:= x3*z2;
     xy23:= x2*y3;
     xy32:= x3*y2;

     a:= y1*z23 - z1*y23 + yz23 - yz32;
     b:= x1*z23 - z1*x23 + xz23 - xz32;
     c:= x1*y23 - y1*x23 + xy23 - xy32;
     d:= x1*(yz23 - yz32) - y1*(xz23-xz32) + z1*(xy23 -xy32);
   END;  { PROCEDURE PLANE }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION TAN
|
|  This FUNCTION finds the tangent of an ANGLE in radians.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE to take Tangent of                     rad
|
|  OutPuts       :
|    TAN         - Result
|
|  Locals        :
|    Temp        - Temporary Real variable
|
 ----------------------------------------------------------------------------- }

FUNCTION TAN                 ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Infinity   : EXTENDED = 999999.9;
     Small      : EXTENDED =     0.000001;
   VAR
     Temp : EXTENDED;
   BEGIN
     Temp := COS( XVal );

     IF ABS( Temp ) < Small THEN
        TAN := Infinity
      ELSE
        TAN := SIN( XVal ) / Temp;
   END;  { FUNCTION TAN }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION COT
|
|  This FUNCTION finds the Cotangent of an ANGLE in radians.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE to take Cotangent of                   rad
|
|  OutPuts       :
|    COT         - Result
|
|  Locals        :
|    Temp        - Temporary Real variable
|
 ----------------------------------------------------------------------------- }

FUNCTION COT                 ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Infinity   : EXTENDED = 999999.9;
     Small      : EXTENDED =     0.000001;
   VAR
     Temp : EXTENDED;
   BEGIN
     Temp:= TAN( XVal );

     IF ABS( Temp ) < Small THEN
         COT := Infinity
       ELSE
         COT := 1.0 / Temp;
   END;  { FUNCTION COT }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION CSC
|
|  This FUNCTION finds the Cosecant of an ANGLE in radians.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE to take Cosecant of                    rad
|
|  OutPuts       :
|    CSC         - Result
|
|  Locals        :
|    Temp        - Temporary Real variable
|
 ----------------------------------------------------------------------------- }

FUNCTION CSC                 ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Infinity   : EXTENDED = 999999.9;
     Small      : EXTENDED =     0.000001;
   VAR
     Temp : EXTENDED;
   BEGIN
     Temp:= SIN( XVal );

     IF ABS( Temp ) < Small THEN
         CSC := Infinity
       ELSE
         CSC := 1.0 / Temp;
   END;  { FUNCTION CSC }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION SEC
|
|  This FUNCTION finds the secant of an ANGLE in radians.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE to take secant of                      rad
|
|  OutPuts       :
|    SEC         - Result
|
|  Locals        :
|    Temp        - Temporary Real variable
|
 ----------------------------------------------------------------------------- }

FUNCTION SEC                 ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Infinity   : EXTENDED = 999999.9;
     Small      : EXTENDED =     0.000001;
   VAR
     Temp : EXTENDED;
   BEGIN
     Temp    := COS( XVal );

     IF ABS( Temp ) < Small THEN
         SEC := Infinity
       ELSE
         SEC := 1.0 / Temp;
   END;  { FUNCTION SEC }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION ATAN2
|
|  This FUNCTION performs the arc tangent 2 FUNCTION which resolves
|    quadrants.  The arguments passed are the sine and cosine values.
|
|  Algorithm     : Determine the quadrant using IF statments
|                  IF the answer is not a sepcial case, 0, 180, etc
|                     find the arctangent
|                  otherwise, find the special case values
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    SinValue    - Sine of desired ANGLE                        rad
|    CosValue    - Cosine of desired value                      rad
|
|  OutPuts       :
|    ATAN2       - Arctangent with resolved quadrants           0.0 to 2Pi rad
|
|  Locals        :
|    TanArg      - Temporary EXTENDED Value
|    Quadrant    - Quadrant of the answer                       1 2 3 4
|    SinINTEGER  - Sign of the value                            +1 or -1
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION ATAN2               ( SinValue,CosValue                     : EXTENDED ) : EXTENDED;
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;    { 6.28318530717959; }
   VAR
     TanArg               : EXTENDED;
     Quadrant, SinINTEGER : INTEGER;
   BEGIN
     Quadrant:= 5;
     IF (SinValue > 0.0 ) and (SinValue < 1.0 ) and
        (CosValue > 0.0 ) and (CosValue < 1.0 ) THEN
          quadrant:= 1;
     IF (SinValue > 0.0 ) and (SinValue < 1.0 ) and
        (CosValue < 0.0 ) and (CosValue >-1.0 ) THEN
          quadrant:= 2;
     IF (SinValue > -1.0 ) and (SinValue < 0.0 ) and
        (CosValue <  0.0 ) and (CosValue >-1.0 ) THEN
          quadrant:= 3;
     IF (SinValue > -1.0 ) and (SinValue < 0.0 ) and
        (CosValue >  0.0 ) and (CosValue < 1.0 ) THEN
          quadrant:= 4;
     IF Quadrant <> 5 THEN
       BEGIN
         tanarg:= ARCTAN(SinValue/CosValue);
         IF (Quadrant <  4) and (Quadrant <> 1) THEN
             tanarg:= tanarg + Pi
           ELSE
             IF Quadrant =  4 THEN
                 tanarg:= tanarg + TwoPi;
       END
       ELSE
         BEGIN                                  { Special Cases }
           SinINTEGER:= Round(SinValue);
           CASE SinINTEGER OF
             -1 :  TanArg:= 3.0*Pi/2.0;
              0 :  IF ROUND(CosValue) > 0.0 THEN
                       TanArg:= 0.0
                     ELSE
                       TanArg:= Pi;
              1 :  TanArg:=  Pi/2.0;
           END; { Case }
         END;
     ATAN2:= tanarg;
   END; { FUNCTION ATAN2 }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION ASIN
|
|  This FUNCTION evaluates Arc Sine using the standard Arc Tangent FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE value                                  -1.0 to 1.0 rad
|
|  OutPuts       :
|    ASIN      - Result                                       -Pi/2 to Pi/2 rad
|
|  Locals        :
|    Temp        - Temporary EXTENDED Value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION ASIN              ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Small      : EXTENDED =     0.000001;
   VAR
     Temp : EXTENDED;
   BEGIN
     IF ABS( XVal ) < 1.0 THEN
         Temp:= ARCTAN( XVal / SQRT(1.0 - XVal*XVal) )
       ELSE
         IF ABS(XVal)-1.0 < Small THEN
             IF XVal > 0.0  THEN
                 Temp:=   Pi / 2.0  { XVal =  1.0 }
               ELSE
                 Temp:=  -Pi / 2.0  { XVal = -1.0 }
           ELSE
             Writeln( 'Error in ASIN argument = ',XVal );

     ASIN:= Temp;
   END;  { FUNCTION ASIN }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION ACOS
|
|  This FUNCTION evaluates Arc Cosine using the ASIN FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE Value                                  -1.0 to 1.0 rad
|
|  OutPuts       :
|    ACOS      - Result                                       0.0 to Pi rad
|
|  Locals        :
|    Temp        - Temporary EXTENDED Value
|
|  Coupling      :
|    ASIN        Sine of an ANGLE in radians
|
 ----------------------------------------------------------------------------- }

FUNCTION ACOS              ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Small      : EXTENDED =     0.000001;
   VAR
     Temp : EXTENDED;
   BEGIN
     IF ABS(XVal) < 1.0 THEN
       BEGIN
         Temp:= ASIN( SQRT(1.0 - XVal*XVal) );
         IF XVal < 0.0 THEN
             Temp:= Pi - Temp;
       END
       ELSE
         IF ABS(XVal)-1.0 < Small THEN
             IF XVal > 0.0  THEN
                 Temp:=  0.0    { XVal =  1.0 }
               ELSE
                 Temp:=  Pi     { XVal = -1.0 }
           ELSE
             Writeln( 'Error in ACOS argument = ',XVal );

     ACOS:= Temp;
   END;  { FUNCTION ACOS }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION COSH
|
|  This FUNCTION evaluates the hyperbolic cosine FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE value                                  any real
|
|  OutPuts       :
|    COSH        - Result                                       1.0 to Infinity
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION COSH                ( XVal                                  : EXTENDED ) : EXTENDED;
   BEGIN
     COSH:= 0.5*( EXP(XVal) + EXP(-XVal) );
   END;  { FUNCTION COSH }



{ ------------------------------------------------------------------------------
|
|                           FUNCTION ACOSH
|
|  This FUNCTION evaluates the inverse hyperbolic cosine FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE Value                                  1.0 to Infinity
|
|  OutPuts       :
|    ACOSH     - Result                                       any real
|
|  Locals        :
|    Temp        - Temporary EXTENDED Value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION ACOSH             ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Undefined  : EXTENDED = 999999.1;
   VAR
     Temp : EXTENDED;
   BEGIN
     IF XVal*XVal - 1.0 < 0.0 THEN
       BEGIN
         Temp:= Undefined;
         WriteLn( 'Error in ACOSH FUNCTION ' );
       END
       ELSE
         Temp:= LN( XVal + SQRT( XVal*XVal - 1.0 ) );

     ACOSH:= Temp;
   END;  { FUNCTION ACOSH }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION SINH
|
|  This FUNCTION evaluates the hyperbolic sine FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE Value                                  any real
|
|  OutPuts       :
|    SINH        - Result                                       any real
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION SINH                ( XVal                                  : EXTENDED ) : EXTENDED;
   BEGIN
     SINH:= 0.5*( EXP(XVal) - EXP(-XVal) );
   END;  { FUNCTION SINH }




{ ------------------------------------------------------------------------------
|
|                           FUNCTION ASINH
|
|  This FUNCTION evaluates the inverse hyperbolic sine FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE Value                                  any real
|
|  OutPuts       :
|    ASINH     - Result                                       any real
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION ASINH             ( XVal                                  : EXTENDED ) : EXTENDED;
   BEGIN

     ASINH:= LN( XVal + SQRT( XVal*XVal + 1.0 ) );

   END;  { FUNCTION ASINH }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION TANH
|
|  This FUNCTION evaluates the hyperbolic tangent FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE Value                                  any real
|
|  OutPuts       :
|    TANH        - Result                                       -1.0 to 1.0
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION TANH                ( XVal                                  : EXTENDED ) : EXTENDED;
   BEGIN

     TANH:= ( EXP(XVal) - EXP(-XVal) ) / ( EXP(XVal) + EXP(-XVal) );

   END;  { FUNCTION TANH }



{ ------------------------------------------------------------------------------
|
|                           FUNCTION ATANH
|
|  This FUNCTION evaluates the inverse hyperbolic tangent FUNCTION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    XVal        - ANGLE Value                                  -1.0 to 1.0
|
|  OutPuts       :
|    ATANH     - Result                                       any real
|
|  Locals        :
|    Temp        - Temporary EXTENDED Value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION ATANH             ( XVal                                  : EXTENDED ) : EXTENDED;
   CONST
     Small    : EXTENDED = 0.000001;
     Undefined: EXTENDED = 999999.1;
   VAR
     Temp : EXTENDED;
   BEGIN
     IF 1.0 - ABS(XVal) < Small THEN
       BEGIN
         Temp:= Undefined;
         WriteLn( 'Error in ATANH FUNCTION ' );
       END
       ELSE
         Temp:= 0.5 * LN( (1.0 + XVal) / (1.0 - XVal) );

     ATANH:= Temp;
   END;  { FUNCTION ATANH }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION DOT
|
|  This FUNCTION finds the DOT product of two vectors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|
|  OutPuts       :
|    DOT         - Result
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION DOT                 ( Vec1,Vec2                             : Vector ) : EXTENDED;
   BEGIN
     DOT:= Vec1[1]*Vec2[1] + Vec1[2]*Vec2[2] + Vec1[3]*Vec2[3];
   END;  { FUNCTION DOT }





{ ------------------------------------------------------------------------------
|
|                           PROCEDURE CROSS
|
|  This PROCEDURE crosses two vectors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|
|  OutPuts       :
|    OutVec      - Vector result of A x B
|
|  Locals        :
|    None.
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE CROSS              ( Vec1,Vec2                             : Vector;
                               VAR OutVec                            : Vector );
   BEGIN
     OutVec[1]:= Vec1[2]*Vec2[3]-Vec1[3]*Vec2[2];
     OutVec[2]:= Vec1[3]*Vec2[1]-Vec1[1]*Vec2[3];
     OutVec[3]:= Vec1[1]*Vec2[2]-Vec1[2]*Vec2[1];

     MAG( OutVec );
   END;   { PROCEDURE CROSS }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MAG
|
|  This PROCEDURE finds the magnitude of a vector.  The tolerance is set to
|    0.000001, thus the 1.0E-12 for the squared test of underflows.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec       - Vector
|
|  OutPuts       :
|    Vec       - Answer stored in fourth component
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE MAG                ( VAR Vec                               : Vector );
   VAR Temp: EXTENDED;
   BEGIN
     Temp:= Vec[1]*Vec[1] + Vec[2]*Vec[2] + Vec[3]*Vec[3];

     IF ABS( Temp ) >= 1.0E-12 THEN
         Vec[4]:= SQRT( Temp )
       ELSE
         Vec[4]:= 0.0;
   END;  { PROCEDURE MAG }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE NORM
|
|  This PROCEDURE calculates a unit vector given the original vector.  IF a
|    zero vector is input, the vector is set to zero.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec         - Vector
|
|  OutPuts       :
|    OutVec      - Unit Vector
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE NORM               ( Vec                                   : Vector;
                               VAR OutVec                            : Vector );
   CONST
     Small : EXTENDED = 0.000001;
   VAR
     i     : INTEGER;
   BEGIN
     MAG( Vec );
     IF Vec[4] > Small THEN
         FOR i:= 1 to 4 DO
             OutVec[i]:= Vec[i]/Vec[4]
       ELSE
         FOR i:= 1 to 4 DO
             OutVec[i]:= 0.0;

   END;  { PROCEDURE NORM }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ROTi
|
|  This PROCEDURE performs a rotation about the ith axis. i is specified
|    for each operation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec         - Input vector
|    XVal        - ANGLE of rotation              rad
|
|  OutPuts       :
|    OutVec      - Vector result
|
|  Locals        :
|    c           - Cosine of the ANGLE XVal
|    s           - Sine of the ANGLE XVal
|    Temp        - Temporary EXTENDED value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE ROT1               ( Vec                                   : Vector;
                               XVal                                  : EXTENDED;
                               VAR OutVec                            : Vector );
   VAR
     c, s, Temp : EXTENDED;
   BEGIN
     Temp:= Vec[3];
     c:= COS( XVal );
     s:= SIN( XVal );

     OutVec[3]:= c*Vec[3] - s*Vec[2];
     OutVec[2]:= c*Vec[2] + s*Temp;
     OutVec[1]:= Vec[1];
     OutVec[4]:= Vec[4];
   END;  { PROCEDURE ROT1 }


PROCEDURE ROT2               ( Vec                                   : Vector;
                               XVal                                  : EXTENDED;
                               VAR OutVec                            : Vector );
   VAR
     c, s, Temp : EXTENDED;
   BEGIN
     Temp:= Vec[3];
     c:= COS( XVal );
     s:= SIN( XVal );

     OutVec[3]:= c*Vec[3] + s*Vec[1];
     OutVec[1]:= c*Vec[1] - s*Temp;
     OutVec[2]:= Vec[2];
     OutVec[4]:= Vec[4];
   END;  { PROCEDURE ROT2 }


PROCEDURE ROT3               ( Vec                                   : Vector;
                               XVal                                  : EXTENDED;
                               VAR OutVec                            : Vector );
   VAR
     c, s, Temp : EXTENDED;
   BEGIN
     Temp:= Vec[2];
     c:= COS( XVal );
     s:= SIN( XVal );

     OutVec[2]:= c*Vec[2] - s*Vec[1];
     OutVec[1]:= c*Vec[1] + s*Temp;
     OutVec[3]:= Vec[3];
     OutVec[4]:= Vec[4];
   END;  { PROCEDURE ROT3 }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ADDVEC
|
|  This PROCEDURE adds two vectors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|
|  OutPuts       :
|    OutVec      - Vector result of A + B
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE ADDVEC             ( Vec1,Vec2                             : Vector;
                               VAR OutVec                            : Vector );
   VAR
     i : INTEGER;
   BEGIN
     FOR i:=1 to 3 DO
         OutVec[i]:= Vec1[i] + Vec2[i];
     MAG( OutVec );
   END;  { PROCEDURE ADDVEC }


{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ADD3VEC
|
|  This PROCEDURE adds three vectors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|    Vec3        - Vector number 3
|
|  OutPuts       :
|    OutVec      - Vector result of Vec1 + Vec2 + Vec3
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE ADD3VEC            ( Vec1,Vec2,Vec3                        : Vector;
                               VAR OutVec                            : Vector );
   VAR
     i : INTEGER;
   BEGIN
     FOR i:=1 to 3 DO
         OutVec[i]:= Vec1[i] + Vec2[i] + Vec3[i];
     MAG( OutVec );
   END;  { PROCEDURE ADD3VEC }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LNCOM1
|
|  This PROCEDURE calculates the linear combination of a vector
|    multiplied by a constants.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    A1          - constant number
|    Vec         - Vector number
|
|  OutPuts       :
|    OutVec      - Vector result of A1*Vec1 + A2*Vec2
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE LNCOM1             ( A                                     : EXTENDED;
                               Vec                                   : Vector;
                               VAR OutVec                            : Vector );
   VAR
     i : INTEGER;
   BEGIN
     FOR i:= 1 to 3 DO
         OutVec[i]:= A*Vec[i];
     MAG( OutVec );
   END;  { PROCEDURE LNCOM1 }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LNCOM2
|
|  This PROCEDURE calculates the linear combination of two vectors
|    multiplied by two different constants.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    A1          - constant number 1
|    A2          - constant number 2
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|
|  OutPuts       :
|    OutVec      - Vector result of A1*Vec1 + A2*Vec2
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE LNCOM2             ( A1,A2                                 : EXTENDED;
                               Vec1,Vec2                             : Vector;
                               VAR OutVec                            : Vector );
   VAR
     i : INTEGER;
   BEGIN
     FOR i:= 1 to 3 DO
         OutVec[i]:= a1*Vec1[i] + a2*Vec2[i];
     MAG( OutVec );
   END;  { PROCEDURE LNCOM2 }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LNCOM3
|
|  This PROCEDURE calculates the linear combination of three vectors
|    multiplied by three different constants.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    A1          - constant number 1
|    A2          - constant number 2
|    A3          - constant number 3
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|    Vec3        - Vector number 3
|
|  OutPuts       :
|    OutVec      - Vector result of A1*Vec1 + A2*Vec2 + A3*Vec3
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    MAG           Magnitude of a vector
|
 ----------------------------------------------------------------------------- }

PROCEDURE LNCOM3             ( A1,A2,A3                              : EXTENDED;
                               Vec1,Vec2,Vec3                        : Vector;
                               VAR OutVec                            : Vector );
   VAR
     i : INTEGER;
   BEGIN
     FOR i:= 1 to 3 DO
         OutVec[i]:= a1*Vec1[i] + a2*Vec2[i] + a3*Vec3[i];
     MAG( OutVec );
   END;  { PROCEDURE LNCOM3 }
{}
{ ---- This PROCEDURE takes an arbitrary number of points and
|  fits a polynomial of degree <degree>. The datapoints are stored in a
|  matrix having rows <NumPts> by 2 <one for x, and y>.
|  See Matthews pg 230-232.
|
|    Assumes the form of
|       ax2 + bx + c = y
|    coeff indices are
|       1 = a, 2 = b, 3 = c
|    MIN is at
|       2ax + b = 0
|    y is found by plugging a back into the polynomial
|
|  dav  3 Apr 97
  ---- }

PROCEDURE POLYFIT            ( Degree,NumPts                         : INTEGER;
                               DataPoints                            : Matrix;
                               VAR Coeff                             : Matrix;
                               VAR MinX,MinY                         : EXTENDED );
   VAR
     j,k,r,c : INTEGER;
     ai, a, b,parr : matrix;
     p : EXTENDED;
   BEGIN
     b:= NIL;
     a:= NIL;
     ai:= NIL;
     parr:= NIL;
     INITMATRIX( Degree+1,1,1,1, b );
     INITMATRIX( Degree+1,Degree+1,1,1, a );
     INITMATRIX( 2*Degree,1,1,1, Parr );

     { ---- Find the sum of the product terms (x*y) ---- }
     FOR k:= 1 to NumPts DO
       BEGIN
         p:= 1;
         FOR r:= 1 to Degree+1 DO
           BEGIN
             ASSIGNVAL(b,r,1, GETVAL(b,r,1) + GETVAL(DataPoints,k,2)*p );
             p:= p*GETVAL(DataPoints,k,1);
           END;
       END;

     { ---- Find the sum of powers for x (x**) ---- }
     FOR k:= 1 to NumPts DO
       BEGIN
         p:= GETVAL(DataPoints,k,1);
         FOR j:= 1 to 2*Degree DO
           BEGIN
             ASSIGNVAL( parr,j,1,  GETVAL(parr,j,1) + p );
             p:= p*GETVAL(DataPoints,k,1);
           END;
       END;

     { ---- Find the matrix entries for the equations ---- }
     FOR r:= 1 to Degree+1 DO
         FOR c:= 1 to Degree+1 DO
             IF r+c <> 2 THEN
                 ASSIGNVAL( a,r,c,  GETVAL(parr,r+c-2,1) )
               ELSE
                 ASSIGNVAL( a,r,c,  NumPts );

     { ---- Solve linear equations for coeffeicients ---- }
     MATINVERSE( A      , 3         , AI );
     MATMULT   ( AI,b, 3,3,1     , coeff );

     { ---- Find minimum values ---- }
     MinX:= -GETVAL(coeff,2,1) / (2*GETVAL(coeff,3,1));
     MinY:= GETVAL(coeff,3,1)*MinX*MinX + GETVAL(coeff,2,1)*MinX + GETVAL(coeff,1,1);

     DELMATRIX( a );
     DELMATRIX( ai );
     DELMATRIX( b );
     DELMATRIX( parr );
   END;  { PROCEDURE POLYFIT }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ANGLE
|
|  This PROCEDURE calculates the ANGLE between two vectors.  The output is
|    set to 999999.1 to indicate an undefined value.  Be SURE to check for
|    this at the output phase.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Vec1        - Vector number 1
|    Vec2        - Vector number 2
|
|  OutPuts       :
|    Theta       - ANGLE between the two vectors  -Pi to Pi
|
|  Locals        :
|    Temp        - Temporary REAL variable
|
|  Coupling      :
|    DOT           DOT Product of two vectors
|    ACOS        Arc Cosine FUNCTION
|
 ----------------------------------------------------------------------------- }

PROCEDURE ANGLE              ( Vec1,Vec2                             : Vector;
                               VAR Theta                             : EXTENDED );
   CONST
     Small    : EXTENDED = 0.000001;
     Undefined: EXTENDED = 999999.1;
   VAR
     Temp : EXTENDED;
   BEGIN
     IF Vec1[4]*Vec2[4] > SQR(Small) THEN
       BEGIN
         Temp:= DOT(Vec1,Vec2) / (Vec1[4]*Vec2[4]);
         IF ABS( Temp ) > 1.0 THEN
             Temp:= SGN(Temp) * 1.0;
         Theta:= ACOS( Temp );
       END
       ELSE
         Theta:= Undefined;
   END;  { PROCEDURE ANGLE   }

{}
PROCEDURE DMulRSub           ( VAR ALPR,ALPI                         : Vector;
                               BETR,BETI                             : Vector );
   VAR
     Te1,Te2,Te3,Te4,Te5,Te6,Te7,Te8,Te9,Te10,Te11,Te12,
     Te13,Te14,Te15,Te16,TeM,DE15,DE16,TemTe7,TemTe8 : EXTENDED;
   BEGIN
      Te1:= ALPR[1]-ALPR[3];
      Te2:= ALPI[1]-ALPI[3];
      Te5:= ALPR[3]-ALPR[2];
      Te6:= ALPI[3]-ALPI[2];
      TeM:= Te5*Te5+Te6*Te6;

      { ---------------- Check for zero values of tem -------------- }
      IF ABS( Tem ) > 1.0E-20 THEN
        BEGIN
          Te3:= (Te1*Te5+Te2*Te6)/TeM;
          Te4:= (Te2*Te5-Te1*Te6)/TeM;
        END
        ELSE
        BEGIN
          Te3:= 0.0;
          Te4:= 0.0;
        END;

      Te7 := Te3+1.0;
      Te9 := Te3*Te3-Te4*Te4;
      Te10:= 2.0*Te3*Te4;
      DE15:= Te7*BETR[3]-Te4*BETI[3];
      DE16:= Te7*BETI[3]+Te4*BETR[3];
      Te11:= Te3*BETR[2]-Te4*BETI[2]+BETR[1]-DE15;
      Te12:= Te3*BETI[2]+Te4*BETR[2]+BETI[1]-DE16;

      Te7 := Te9-1.0;
      Te1 := Te9*BETR[2]-Te10*BETI[2];
      Te2 := Te9*BETI[2]+Te10*BETR[2];
      Te13:= Te1-BETR[1]-Te7*BETR[3]+Te10*BETI[3];
      Te14:= Te2-BETI[1]-Te7*BETI[3]-Te10*BETR[3];
      Te15:= DE15*Te3-DE16*Te4;
      Te16:= DE15*Te4+DE16*Te3;

      Te1:= Te13*Te13-Te14*Te14-4.0*(Te11*Te15-Te12*Te16);
      Te2:= 2.0*Te13*Te14-4.0*(Te12*Te15+Te11*Te16);

{ --------------------------------------------------------------------
|   Sometimes, for stiff systems (the roots vary widely in order
|   of magnitude), Te1 and Te2 get large enough to have their
|   squares overflow the floating point range.  To prevent this,
|   when either one is large, they are scaled by 10**10 for the
|   purpose of finding TeM.  The scale is restored when the
|   magnitude computation is completed.  This should not affect
|   the accuracy of the computations, since the mantissa is not
|   affected, only the exponent.
  -------------------------------------------------------------------- }

      IF ( Te1 > 1.0E15 ) or ( Te2 > 1.0E15 ) THEN
        BEGIN
          Te1:= Te1*1.0E-10;
          Te2:= Te2*1.0E-10;
          TeM:= 1.0E10*SQRT(Te1*Te1+Te2*Te2);
        END
        ELSE
          TeM:= SQRT(Te1*Te1+Te2*Te2);

      IF Te1 > 0.0 THEN
        BEGIN
          Te3:= SQRT(0.5*(TeM+Te1));
          IF Te2 < 0.0 THEN
              Te3:= -Te3;
     { ----------------- check for zero values of te3 -------------- }
          IF ABS( Te3 ) < 1.0E-15 THEN
              Te4:= 0.0
            ELSE
              Te4:= 0.5*Te2/Te3;
        END
        ELSE
        BEGIN
          Te4:= SQRT(0.5*(TeM-Te1));
     { --------------------- Check for underflows ------------------ }
          IF ABS( Te4 ) < 1.0E-15 THEN
              Te3:= 0.0
            ELSE
              Te3:= 0.5*Te2/Te4;
        END;
{}
      Te7 := Te13+Te3;
      Te8 := Te14+Te4;
      Te9 := Te13-Te3;
      Te10:= Te14-Te4;
      Te1 := 2.0*Te15;
      Te2 := 2.0*Te16;

      IF (Te7*Te7+Te8*Te8-Te9*Te9-Te10*Te10) <= 0.0 THEN
        BEGIN
          Te7:= Te9;
          Te8:= Te10;
        END;
      TeM:= Te7*Te7+Te8*Te8;

      TemTe7:= Tem*Te7;
      TemTe8:= Tem*Te8;

      { ------------- Check for values of almost zero -------------- }
      IF ABS( TemTe7 ) < 1.0E-20 THEN
        BEGIN
          Te3:= 0.0;
          Te4:= 0.0;
        END
        ELSE
        BEGIN
          Te3:= Te1/TeM*Te7;
          Te4:= Te2/TeM*Te7;
        END;
      IF ABS( TemTe8 ) > 1.0E-20 THEN
        BEGIN
          Te3:= Te3 + Te2/TeM*Te8;
          Te4:= Te4 - Te1/TeM*Te8;
        END;

      ALPR[4]:= ALPR[3]+Te3*Te5-Te4*Te6;
      ALPI[4]:= ALPI[3]+Te3*Te6+Te4*Te5;
  END;  { PROCEDURE DMulrsub }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE FACTOR
|
|  This PROCEDURE is a root finding algorithm.  It takes the polynomial and
|    returns the roots (real and imaginary) in the Roots array.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Poly        - Array of 16 coefficients
|                    representing the polynomial
|                    [1] is x^8th, [2] is x^7th, ...
|                    others are zero
|    NRoots      - Number of roots
|
|  OutPuts       :
|    Roots       - Array containing roots (real,imag)
|
|  Locals        :
|                -
|                -
|                -
|
|  Coupling      :
|    DMulRSub    -
|
|  References    :
|    Original FORTRAN code from USAFA/DFAS MiniTotal program, author unknown
|    This is Bairstows method?
|
 ----------------------------------------------------------------------------- }

PROCEDURE FACTOR             ( Poly                                  : Array16;
                               NRootS                                : INTEGER;
                               VAR RootS                             : Array15by2 );
   CONST
     Small : EXTENDED = 0.000001;
   VAR
     DPoly               : Array16;
     AlpR,Alpi,BetR,Beti : Vector;
     Skip                : Boolean;

     Mode,LoopCnt,kk,i,j,l,RootCnt : INTEGER;
     Temp1,    Temp2,    AXR, AXi, PMax, Tem1, Tem2,
     TempReal, TempImag, Temp7     : EXTENDED;
   BEGIN
      PMax:= 0.0;
      FOR KK:= 1 to NRootS+1 DO
         IF ABS(Poly[KK]) > PMax THEN
             PMax:= Poly[KK];

      IF ABS(PMax) < Small THEN
          PMax:= 1.0;
    
      FOR KK:= 1 to NRootS+1 DO
         DPoly[kk]:= Poly[kk]/PMax;

      IF NRootS > 0 THEN
        BEGIN
          RootCnt:= 0;
          i:= NRootS+1;

          WHILE (ABS(DPoly[i]) < Small) and (RootCnt <> NRoots) DO
            BEGIN
              RootCnt:= RootCnt+1;
              RootS[RootCnt,1]:= 0.0;
              RootS[RootCnt,2]:= 0.0;
              i:= i-1;
            END; { While }

          IF RootCnt <> NRootS THEN
            BEGIN
              AXR:= 0.8;
              AXi:= 0.0;
              L  := 1;
              LOOPCNT:= 1;
              AlpR[1]:= AXR;
              Alpi[1]:= AXi;
              MODE:= 1;

              WHILE RootCnt < NRoots DO
                BEGIN
                  BetR[4]:= DPoly[1];
                  Beti[4]:= 0.0;
                  FOR i:= 1 to NRootS DO
                    BEGIN
                      J:= i+1;
                      Temp1:= BetR[4]*AXR - Beti[4]*AXi;
                      Beti[4]:= Beti[4]*AXR + BetR[4]*AXi;
                      BetR[4]:= Temp1 + DPoly[J];
                     END;

                  TempReal:= BetR[4];
                  Tempimag:= Beti[4];

                  IF RootCnt <> 0 THEN
                    BEGIN
                      FOR i:= 1 to RootCnt DO
                        BEGIN
                          Tem1   := AXR - RootS[i,1];
                          Tem2   := AXi - RootS[i,2];
                          Temp1  := Tem1*Tem1 + Tem2*Tem2;
                          Temp2  := (BetR[4]*Tem1 + Beti[4]*Tem2)/Temp1;
                          Beti[4]:= (Beti[4]*Tem1 - BetR[4]*Tem2)/Temp1;
                          BetR[4]:= Temp2;
                        END;
                    END;

                  CASE Mode OF
                    1 : BEGIN
                          BetR[1]:= BetR[4];
                          Beti[1]:= Beti[4];
                          AXR    := 0.85;
                          AlpR[2]:= AXR;
                          Alpi[2]:= AXi;
                          MODE   := 2;
                        END;
                    2 : BEGIN
                          BetR[2]:= BetR[4];
                          Beti[2]:= Beti[4];
                          AXR    := 0.9;
                          AlpR[3]:= AXR;
                          Alpi[3]:= AXi;
                          MODE   := 3;
                        END;
                    3 : BEGIN
                          BetR[3]:= BetR[4];
                          Beti[3]:= Beti[4];
                          DMULRSUB( AlpR,Alpi,BetR,Beti );
                          AXR    := AlpR[4];
                          AXi    := Alpi[4];
                          MODE   := 4;
                        END;
                    5 : BEGIN
                          BetR[1]:=  BetR[4];
                          Beti[1]:=  Beti[4];
                          AXR    :=  AlpR[2];
                          AXi    := -Alpi[2];
                          Alpi[2]:= -Alpi[2];
                          MODE   :=  6;
                        END;
                    6 : BEGIN
                          BetR[2]:=  BetR[4];
                          Beti[2]:=  Beti[4];
                          AXR    :=  AlpR[3];
                          AXi    := -Alpi[3];
                          Alpi[3]:= -Alpi[3];
                          L      :=  2;
                          MODE   :=  3;
                        END;

     { -------------------  the convergence mode ------------------- }
                    4 : BEGIN
                          Skip:= FALSE;
                          IF ABS(TempReal)+ABS(Tempimag) > 1.0E-20 THEN
                              Temp7:= ABS(AlpR[3]-AXR)+ABS(Alpi[3]-AXi);
                          IF Temp7/(ABS(AXR)+ABS(AXi)) > 1.0E-7 THEN
                            BEGIN
                              iNC(LOOPCNT);
                              FOR i:= 1 to 3 DO
                                BEGIN
                                  AlpR[i]:= AlpR[i+1];
                                  Alpi[i]:= Alpi[i+1];
                                  BetR[i]:= BetR[i+1];
                                  Beti[i]:= Beti[i+1];
                                END;
                              IF LOOPCNT < 100 THEN
                                BEGIN
                                  DMULRSUB( AlpR,Alpi,BetR,Beti );
                                  AXR := AlpR[4];
                                  AXi := Alpi[4];
                                  MODE:= 4;
                                  Skip:= TRUE;
                                END;
                            END;

                          IF NOT Skip THEN
                            BEGIN
                              RootCnt:= RootCnt+1;
                              RootS[RootCnt,1]:= AlpR[4];
                              RootS[RootCnt,2]:= Alpi[4];
                              LOOPCNT:= 0;

                              IF RootCnt < NRootS THEN
                                BEGIN
                                  IF ABS(RootS[RootCnt,2]) > Small THEN
                                    BEGIN
                                      IF L = 1 THEN
                                        BEGIN
                                          AXR    :=  AlpR[1];
                                          AXi    := -Alpi[1];
                                          Alpi[1]:= -Alpi[1];
                                          MODE   :=  5;
                                        END
                                        ELSE
                                        BEGIN
                                          AXR:= 0.8;
                                          AXi:= 0.0;
                                          L  := 1;
                                          LOOPCNT:= 1;
                                          AlpR[1]:= AXR;
                                          Alpi[1]:= AXi;
                                          MODE   := 1;
                                        END;
                                    END
                                    ELSE
                                    BEGIN
                                      AXR:= 0.8;
                                      AXi:= 0.0;
                                      L  := 1;
                                      LOOPCNT:= 1;
                                      AlpR[1]:= AXR;
                                      Alpi[1]:= AXi;
                                      MODE   := 1;
                                    END;
                                END;
                            END;  { IF Skip }
                        END;
                  END;  { Case }

                END;  { While }

          END;  { IF RootCnt <> NRoots }

        END;  { IF nRoots > 0 }

   END;  { PROCEDURE FACTOR }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE QUADRATIC
|
|  This PROCEDURE solves for the two Roots of a QUADRATIC equation.  There are
|    no restrictions on the coefficients, and imaginary results are passed
|    out as separate values.  The general form is y = ax2 + bx + c.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    a           - Coefficient of x squared term
|    b           - Coefficient of x term
|    c           - Constant
|
|  OutPuts       :
|    R1r         - Real portion of Root 1
|    R1i         - Imaginary portion of Root 1
|    R2r         - Real portion of Root 2
|    R2i         - Imaginary portion of Root 2
|
|  Locals        :
|    Discrim     - Discriminate b2 - 4ac
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 974
|
 ----------------------------------------------------------------------------- }

PROCEDURE QUADRATIC          ( a,b,c                                 : EXTENDED;
                               VAR R1r,R1i,R2r,R2i                   : EXTENDED  );
   VAR
     Discrim : EXTENDED;
   BEGIN
     { ----------------------   Initialize   ----------------------- }
     R1r:= 0.0;
     R1i:= 0.0;
     R2r:= 0.0;
     R2i:= 0.0;

     Discrim:= b*b - 4.0*a*c;

     { ----------------------  Real Roots  ------------------------- }
     IF Discrim > 0.0 THEN
       BEGIN
         R1r:= ( -b + SQRT(Discrim) ) / ( 2.0*a );
         R2r:= ( -b - SQRT(Discrim) ) / ( 2.0*a );
       END
       ELSE
     { --------------------- Complex Roots ------------------------- }
       BEGIN
         R1r:= -b / ( 2.0*a );
         R2r:= R1r;
         R1i:=  SQRT(-Discrim) / ( 2.0*a );
         R2i:= -SQRT(-Discrim) / ( 2.0*a );
       END;
   END;  { PROCEDURE QUADRATIC }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE CUBIC
|
|  This PROCEDURE solves for the three Roots of a CUBIC equation.  There are
|    no restrictions on the coefficients, and imaginary results are passed
|    out as separate values.  The general form is y = ax3 + bx2 + cx + d.  Note
|    that R1i will ALWAYS be ZERO since there is ALWAYS at least one REAL Root.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    a           - Coefficient of x cubed term
|    b           - Coefficient of x squared term
|    c           - Coefficient of x term
|    d           - Constant
|
|  OutPuts       :
|    R1r         - Real portion of Root 1
|    R1i         - Imaginary portion of Root 1
|    R2r         - Real portion of Root 2
|    R2i         - Imaginary portion of Root 2
|    R3r         - Real portion of Root 3
|    R3i         - Imaginary portion of Root 3
|
|  Locals        :
|    Temp1       - Temporary value
|    Temp2       - Temporary value
|    Root1       - Temporary value of the Root
|    Root2       - Temporary value of the Root
|    Root3       - Temporary value of the Root
|    P           - Coefficient of x squared term where x cubed term is 1.0
|    Q           - Coefficient of x term where x cubed term is 1.0
|    R           - Coefficient of constant term where x cubed term is 1.0
|    Delta       - Discriminator for use with Cardans formula
|    E0          - ANGLE holder for trigonometric solution
|    Phi         - ANGLE used in trigonometric solution
|    CosPhi      - Cosine of Phi
|    SinPhi      - Sine of Phi
|
|  Coupling      :
|    ATAN2         Arctangent including check for 180-360 deg
|    POWER         Raise a base to an exponent
|
|  References    :
|    Vallado       2007, 975
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE CUBIC              ( a,b,c,d                                : EXTENDED;
                               VAR R1r,R1i,R2r,R2i,R3r,R3i            : EXTENDED  );
   CONST
     Rad      : EXTENDED = 180.0/pi; { 57.29577951308230; }
     OneThird : EXTENDED = 1.0/3.0;
     Small    : EXTENDED = 0.000001;
   VAR
     Temp1, Temp2, Root1, Root2, Root3, P, Q, R, Delta,
     E0, CosPhi, SinPhi, Phi : EXTENDED;
   BEGIN
     { ----------------------   Initialize   ----------------------- }
     R1r  := 0.0;
     R1i  := 0.0;
     R2r  := 0.0;
     R2i  := 0.0;
     R3r  := 0.0;
     R3i  := 0.0;
     Root1:= 0.0;
     Root2:= 0.0;
     Root3:= 0.0;

     { ------------ Force coefficients into std form --------------- }
     P:= B/A;
     Q:= C/A;
     R:= D/A;

     a:= OneThird*( 3.0*Q - P*P );
     b:= (1.0/27.0)*( 2.0*P*P*P - 9.0*P*Q + 27.0*R );

     Delta:= (a*a*a/27.0) + (b*b*0.25);

     { ------------------- Use Cardans formula --------------------- }
     IF Delta > Small THEN
       BEGIN
         Temp1:= (-b*0.5)+SQRT(Delta);
         Temp2:= (-b*0.5)-SQRT(Delta);
         IF ABS(Temp1) > Small THEN
             Temp1:= POWER( Temp1,OneThird );
         IF ABS(Temp2) > Small THEN
             Temp2:= POWER( Temp2,OneThird );
         Root1:= Temp1 + Temp2;
         Root2:= -0.5*(Temp1 + Temp2);
         Root3:= -0.5*(Temp1 + Temp2);
         R2i:= -0.5*SQRT( 3.0 )*(Temp1 - Temp2);
         R3i:= -R2i;
       END
       ELSE
     { ---------------- Evaluate zero point ------------------------ }
       BEGIN
         IF ABS( Delta ) < Small  THEN
           BEGIN
             IF ABS(b) > Small THEN
               BEGIN
                 Root1:= -2.0*POWER( b*0.5,OneThird );
                 Root2:=  POWER( b*0.5,OneThird );
                 Root3:= Root2;
               END;
           END
           ELSE
     { ---------------- Use trigonometric identities --------------- }
           BEGIN
             E0     := 2.0*SQRT(-a*OneThird);
             CosPhi := (-b/(2.0*SQRT(-a*a*a/27.0)) );
             SinPhi := SQRT( 1.0-CosPhi*CosPhi );
             Phi    := ATAN2( SinPhi,CosPhi );
             Root1:= E0*COS( Phi*OneThird );
             Root2:= E0*COS( Phi*OneThird + 120.0/Rad );
             Root3:= E0*COS( Phi*OneThird + 240.0/Rad );
           END;
       END;

     R1r:= Root1 - P*OneThird;
     R2r:= Root2 - P*OneThird;
     R3r:= Root3 - P*OneThird;
   END;  { PROCEDURE CUBIC }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE QUARTIC
|
|  This PROCEDURE solves for the four Roots of a QUARTIC equation.  There are
|    no restrictions on the coefficients, and imaginary results are passed
|    out as separate values.  The general form is y = ax4 + bx3 + cx2 + dx + e.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    a           - Coeficient of x fourth term
|    b           - Coefficient of x cubed term
|    c           - Coefficient of x squared term
|    d           - Coefficient of x term
|    e           - Constant
|
|  OutPuts       :
|    R1r         - Real portion of Root 1
|    R1i         - Imaginary portion of Root 1
|    R2r         - Real portion of Root 2
|    R2i         - Imaginary portion of Root 2
|    R3r         - Real portion of Root 3
|    R3i         - Imaginary portion of Root 3
|    R4r         - Real portion of Root 4
|    R4i         - Imaginary portion of Root 4
|
|  Locals        :
|    Temp1       - Temporary value
|    Temp2       - Temporary value
|    Root1       - Temporary value of the Root
|    Root2       - Temporary value of the Root
|    Root3       - Temporary value of the Root
|    s           - alternate variable
|    h           - Temporary value
|    hSqr        - h squared
|    hCube       - h Cubed
|    P           - Term in auxillary equation
|    Q           - Term in auxillary equation
|    R           - Term in auxillary equation
|    Delta       - Discriminator for use with Cardans formula
|    E0          - ANGLE holder for trigonometric solution
|    Phi         - ANGLE used in trigonometric solution
|    CosPhi      - Cosine of Phi
|    SinPhi      - Sine of Phi
|    RPrime      - Values of Roots before final work
|    Temp        - Temporary variable in finding MAX RPrime
|    Eta         - Constant coefficient in QUADRATIC solutions
|    Beta        - Constant coefficient in QUADRATIC solutions
|
|  Coupling      :
|    ATAN2         Arctangent including check for 180-360 deg
|    POWER         Raise a base to an exponent
|    QUADRATIC     Find roots of a QUADRATIC polynomial
|
|  References    :
|    Vallado       2007, 976
|
 ----------------------------------------------------------------------------- }

PROCEDURE QUARTIC            ( a,b,c,d,e                               : EXTENDED;
                               VAR R1r,R1i,R2r,R2i,R3r,R3i,R4r,R4i     : EXTENDED  );
   CONST
     Rad      : EXTENDED = 180.0 / pi;  { 57.29577951308230; }
     OneThird : EXTENDED = 1.0/3.0;
     Small    : EXTENDED = 0.000001;
   VAR
     Temp1, Temp2, Root1, Root2, Root3, s, h, P, Q, R, Delta, E0,
     CosPhi, SinPhi, Phi, RPrime, hSqr, HCube, Eta, Beta, Temp : EXTENDED;
   BEGIN
     { ----------------------   Initialize   ----------------------- }
     R1r  := 0.0;
     R1i  := 0.0;
     R2r  := 0.0;
     R2i  := 0.0;
     R3r  := 0.0;
     R3i  := 0.0;
     R4r  := 0.0;
     R4i  := 0.0;
     Root1:= 0.0;
     Root2:= 0.0;
     Root3:= 0.0;
     { ------------ Force coefficients into std form --------------- }
     b:= B/A;
     c:= C/A;
     d:= D/A;
     e:= E/A;

     H:= -b/4;
     HSqr:= SQR( H );
     HCube:= HSqr * H;

     P:=                      6.0*HSqr   + 3.0*b*h + c;
     Q:=          4.0*HCube + 3.0*b*HSqr + 2.0*c*h + d;
     R:= h*HCube +  b*HCube +   c*HSqr   +    d*h  + e;

     a:=  OneThird*( -P*P-12.0*R );
     b:=  (1.0/27.0)*( -2.0*P*P*P+72.0*P*R-27.0*Q*Q );
     s:= -2.0*OneThird*P;

     Delta:= (a*a*a/27.0) + (b*b*0.25);

     IF ABS(Q) > Small THEN
       BEGIN
     { ------------------- Use Cardans formula --------------------- }
         IF Delta > Small THEN
           BEGIN
             Temp1:= (-b*0.5)+SQRT(Delta);
             Temp2:= (-b*0.5)-SQRT(Delta);
             IF ABS(Temp1) > Small THEN
                 Temp1:= POWER( Temp1,OneThird );
             IF ABS(Temp2) > Small THEN
                 Temp2:= POWER( Temp2,OneThird );
             Root1:= Temp1 + Temp2;
             Root2:= -0.5*(Temp1 + Temp2);
             Root3:= -0.5*(Temp1 + Temp2);
             R2i:= -0.5*SQRT( 3.0 )*(Temp1 - Temp2);
             R3i:= -R2i;
           END
           ELSE
     { ---------------- Evaluate zero point ------------------------ }
           BEGIN
             IF ABS( Delta ) < Small  THEN
               BEGIN
                 IF ABS(b) > Small THEN
                   BEGIN
                     Root1:= -2.0*POWER( b*0.25,OneThird );
                     Root2:=  POWER( b*0.25,OneThird );
                     Root3:= Root2;
                   END;
               END
               ELSE
     { ---------------- Use trigonometric identities --------------- }
               BEGIN
                 E0    := 2.0*SQRT(-a*OneThird);
                 CosPhi:= (-b/(2.0*SQRT(-a*a*a/27.0)) );
                 SinPhi:= SQRT( 1.0-CosPhi*CosPhi );
                 Phi   := ATAN2( SinPhi,CosPhi );
                 Root1:= E0*COS( Phi*OneThird );
                 Root2:= E0*COS( Phi*OneThird + 120.0/Rad );
                 Root3:= E0*COS( Phi*OneThird + 240.0/Rad );
               END;
           END;

     { ---------------- Find largest value of Root ----------------- }
          RPrime:= Root1+s;
          IF (RPrime < Root2+s) and (ABS(R2i)<0.0001) THEN
              RPrime:= Root2+s;
          IF (RPrime < Root3+s) and (ABS(R3i)<0.0001) THEN
              RPrime:= Root3+s;

     { ------ Evaluate coefficients of two resulting Quadratics ---- }
          IF RPrime > Small THEN
            BEGIN
              Eta := 0.5*( P + RPrime - Q/SQRT(RPrime) );
              Beta:= 0.5*( P + RPrime + Q/SQRT(RPrime) );
            END
            ELSE
            BEGIN
              Eta := 0.5*P;
              Beta:= 0.5*P;
              IF RPrime < 0.0 THEN
                  RPrime:= -RPrime;
            END;

          QUADRATIC( 1.0, SQRT(RPrime),Eta, R1r,R1i,R2r,R2i );
          QUADRATIC( 1.0,-SQRT(RPrime),Beta, R3r,R3i,R4r,R4i );

        END { IF Q > Small }
        ELSE
        BEGIN
     { ------- Case where solution reduces to a QUADRATIC ---------- }
          QUADRATIC( 1.0,P,R,   R1r,R1i,R3r,R3i );
          R  := SQRT( R1r*R1r + R1i*R1i );
          IF ABS(R) > Small THEN
              Phi:= ATAN2( R1i/R,R1r/R )
            ELSE
              Phi:= 0.0;
          R1r:= SQRT(R) * COS(Phi*0.5);
          R1i:= SQRT(R) * SIN(Phi*0.5);
          IF ABS(R1i) > Small THEN
              R2r:= R1r
            ELSE
              R2r:= -R1r;
          R2i:= -R1i;

          R  := SQRT( R3r*R3r + R3i*R3i );
          IF ABS(R) > Small THEN
              Phi:= ATAN2( R3i/R,R3r/R )
            ELSE
              Phi:= 0.0;
          R3r:= SQRT(R) * COS(Phi*0.5);
          R3i:= SQRT(R) * SIN(Phi*0.5);
          IF ABS(R3i) > Small THEN
              R4r:= R3r
            ELSE
              R4r:= -R3r;
          R4i:= -R3i;
        END;

     R1r:= R1r + h;
     R2r:= R2r + h;
     R3r:= R3r + h;
     R4r:= R4r + h;
   END;  { PROCEDURE QUARTIC }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE INITMATRIX
|
|  This PROCEDURE initializes the matricies in pascal.  Notice the use of a
|    record structure.  This allows for arrays to be as large as needed,
|    provided memory exists.  Also note each time this is called, NEW is
|    invoked.  Thus, you can have Heap and memory problems IF you don't use
|    DELMATRIX to clear the pointer value!!
|
|  Algorithm     : Loop through the Rows
|                    Loop through the cols
|                      Assign a NEW pointer and all record fields
|                      Build the doubly linked list of pointers
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Rows        - Number of rows for the matrix
|    Cols        - Number of columns for the matrix
|    SRow        - Start Row, allows first row to be 0, etc.
|    SCol        - Start Col, allows first Col to be 0, etc.
|
|  OutPuts       :
|    A           - Matrix to be initialized
|
|  Locals        :
|    i           - Index
|    j           - index
|    NextData    - PTR to next data value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE INITMATRIX         ( Rows,Cols, SRow, SCol                 : INTEGER;
                               VAR A                                 : Matrix );
   VAR
     i,j      : INTEGER;
     NextData : MatrixDataPtr;
   BEGIN
     IF NOT Assigned( A ) THEN
       BEGIN
         NEW( A );
         A^.NumRows:= Rows;
         A^.NumCols:= Cols;
         A^.DPtr   := NIL;
         A^.Head   := NIL;
         A^.Tail   := NIL;

         FOR i:= SRow to Rows DO
           BEGIN
             FOR j:= SCol to Cols DO
               BEGIN
                 New( NextData );
                 With NextData^ Do
                   BEGIN
                     Index := (i-1)*A^.NumCols + j;
                     Number:= 0.0;
                     Next  := NIL;
                     Last  := NIL;
                   END;
                 IF A^.DPtr = NIL THEN
                   BEGIN
                     A^.Head       := NextData;
                     NextData^.Last:= NIL;
                   END
                   ELSE
                   BEGIN
                     NextData^.Last:= A^.Tail;
                     A^.Tail^.Next := NextData;
                   END;
                 A^.Tail      := NextData;
                 A^.Tail^.Next:= NIL;
                 A^.DPtr      := NextData;
               END;
           END;
       END  { IF not already assigned }
       ELSE
{}
       BEGIN { zero out all values already present }
{ Fix this !!!! Is this a bug in PASCAL??
  it seems that local var ptrs have an "assigned value", but no size }
         IF (A^.NumRows > 0 ) And (A^.NumRows < 1000) THEN
          BEGIN
{     WriteLN('deleting old matrix ' );}
             DELMATRIX( A );
           END;

         NEW( A );
         A^.NumRows:= Rows;
         A^.NumCols:= Cols;
         A^.DPtr   := NIL;
         A^.Head   := NIL;
         A^.Tail   := NIL;

         FOR i:= SRow to Rows DO
           BEGIN
             FOR j:= SCol to Cols DO
               BEGIN
                 New( NextData );
                 With NextData^ Do
                   BEGIN
                     Index := (i-1)*A^.NumCols + j;
                     Number:= 0.0;
                     Next  := NIL;
                     Last  := NIL;
                   END;
                 IF A^.DPtr = NIL THEN
                   BEGIN
                     NextData^.Last:= NIL;
                     A^.Head       := NextData;
                   END
                   ELSE
                   BEGIN
                     NextData^.Last:= A^.Tail;
                     A^.Tail^.Next := NextData;
                   END;
                 A^.Tail      := NextData;
                 A^.Tail^.Next:= NIL;
                 A^.DPtr      := NextData;
               END;
           END;
       END;

   END;  { PROCEDURE INITMATRIX }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE DELMATRIX
|
|  This PROCEDURE deletes a matrix in pascal.  Notice the use of a
|    record structure.  It's important to clear the values when no longer
|    needed, or when they will be created again.
|
|  Algorithm     : Start at the head of the pointer list
|                  Loop while not pointing to NIL
|                      Dispose of each pointer
|                  Dispose of the last pointer
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    A           - Matrix to be deleted
|
|  OutPuts       :
|    A           - Former Matrix
|
|  Locals        :
|    i           - Index
|    j           - index
|    Temp        - PTR to data value
|    NextTemp    - PTR to next data value
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE DELMATRIX          ( VAR A                                 : Matrix );
   VAR
     Temp,NextTemp : MatrixDataPtr;
   BEGIN
     IF Assigned( A ) THEN
       BEGIN
         Temp := A^.Head;
         WHILE Temp^.Next <> NIL DO
           BEGIN
             NextTemp:= Temp^.Next;
             DISPOSE( Temp );
             Temp:= NextTemp;
           END;

         DISPOSE( Temp );
         DISPOSE( A );
         A:= NIL;   { Be sure it is set to NIL for future assigned checks }
       END;
   END;  { PROCEDURE DELMATRIX }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION GETVAL
|
|  This FUNCTION gets a value from the record structure.  The FUNCTION is
|    necessary to decode the data.  It's set up to resemble the standard
|    array format, however, ['s are replaced by ('s.
|
|  Algorithm     : Find the index where you desire to get data
|                  Loop forward or backward to the desired index
|                  Assign the value
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    A           - Matrix
|    Rows        - Row number of desired element
|    Cols        - Col number of desired element
|
|  OutPuts       :
|    GetXVal     - Value of the Row,Col point
|
|  Locals        :
|    i           - Index
|    j           - index
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION GETVAL              ( VAR A                                 : Matrix;
                               Row,Col                               : INTEGER ) : EXTENDED;
   VAR
     i,j : INTEGER;
   BEGIN
     j:= (Row-1)*A^.NumCols + Col;

     WHILE ( j > A^.DPtr^.Index ) and ( A^.DPtr^.Next <> NIL ) DO
         A^.DPtr:= A^.DPtr^.Next;

     WHILE ( j < A^.DPtr^.Index ) and ( A^.DPtr^.Last <> NIL ) DO
         A^.DPtr:= A^.DPtr^.Last;

     GETVAL:= A^.DPtr^.Number;

   END;  { FUNCTION GETVAL}
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ASSIGNVAL
|
|  This PROCEDURE assigns a value to the record structure.  This is
|    necessary to decode the data.  It's set up to resemble the standard
|    array format, however, ['s are replaced by ('s.
|
|  Algorithm     : Call GETVAL to get the pointer at the correct index location
|                  Assign the value to the pointer variable record field
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    A           - Matrix
|    Row         - Row number of desired element
|    Col         - Col number of desired element
|    Number      - Value to assign at the desired location
|
|  OutPuts       :
|    A           - Matrix
|
|  Locals        :
|    Temp        -
|
|  Coupling      :
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE ASSIGNVAL          ( VAR A                                 : Matrix;
                               Row,Col                               : INTEGER;
                               Number                                : EXTENDED );
   VAR
     Temp : EXTENDED;
   BEGIN
     Temp:= GETVAL( A,Row,Col );
     A^.DPtr^.Number:= Number;
   END;  { PROCEDURE ASSIGNVAL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MATSCALE
|
|  This PROCEDURE multiplies a matrix by a constant.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat1        - Matrix number 1
|    Scale       - Constant value to multiply by
|
|  OutPuts       :
|    Mat2        - Matrix result of Mat1 * Scale
|
|  Locals        :
|    Row         - Row Index
|    Col         - Column Index
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE MATSCALE           ( Mat1                                  : Matrix;
                               Scale                                 : EXTENDED;
                               VAR Mat2                              : Matrix );
   VAR
     Row,Col : INTEGER;
   BEGIN
     INITMATRIX( Mat1^.NumRows,Mat1^.NumCols,1,1, Mat2 );

     FOR Row:=1 to Mat1^.NumRows DO
         FOR Col:= 1 to Mat1^.NumCols DO
             ASSIGNVAL( Mat2,Row,Col, GETVAL(Mat1,Row,Col)*Scale );
   END;  { PROCEDURE MATSCALE }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MATMULT
|
|  This PROCEDURE multiplies two matricies together.
|
|  Algorithm     : Initialize the pointers for the result matrix
|                  Loop through the Rows
|                     Loop through the Cols
|                        Loop through an index
|                           Multiply and add up each cell value
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat1        - Matrix number 1
|    Mat2        - Matrix number 2
|    Mat1r       - Matrix number 1 rows
|    Mat1c       - Matrix number 1 columns
|    Mat2c       - Matrix number 2 columns
|
|  OutPuts       :
|    Mat3        - Matrix result of Mat1 * Mat2 of size mat1r x mat2c
|
|  Locals        :
|    Row         - Row Index
|    Col         - Column Index
|    ktr         - Index
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE MATMULT            ( Mat1,Mat2                             : Matrix;
                               Mat1r,Mat1c,Mat2c                     : INTEGER ;
                               VAR Mat3                              : Matrix );
   VAR
     Row,Col,ktr : INTEGER;
   BEGIN
     INITMATRIX( Mat1r,Mat2c,1,1, Mat3 );

     FOR Row:=1 to mat1r DO
         FOR Col:= 1 to mat2c DO
           BEGIN
             FOR ktr:= 1 to mat1c DO
                 ASSIGNVAL( Mat3,Row,Col, GETVAL(Mat3,Row,Col)+GETVAL(Mat1,Row,ktr)*
                                      GETVAL(Mat2,ktr,Col) );
           END;
   END;  { PROCEDURE MATMULT }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MATADD
|
|  This PROCEDURE adds two matricies together.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat1        - Matrix number 1
|    Mat2        - Matrix number 2
|    Mat1r       - Matrix number 1 rows
|    Mat1c       - Matrix number 1 columns
|
|  OutPuts       :
|    Mat3        - Matrix result of Mat1 + Mat2
|                    of size mat1r x mat1c
|
|  Locals        :
|    Row         - Row Index
|    Col         - Column Index
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE MATADD             ( Mat1,Mat2                             : Matrix;
                               Mat1r,Mat1c                           : INTEGER ;
                               VAR Mat3                              : Matrix     );
   VAR
     Row,Col  : INTEGER;
   BEGIN
     INITMATRIX( Mat1r,Mat1c,1,1, Mat3 );

     FOR Row := 1 to Mat1r DO

         FOR Col :=1 to Mat1c DO

             ASSIGNVAL( Mat3,Row,Col, GETVAL(Mat1,Row,Col) + GETVAL(Mat2,Row,Col) );

   END;  { PROCEDURE MATADD }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MATSUB
|
|  This PROCEDURE subtracts two matricies together.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat1        - Matrix number 1
|    Mat2        - Matrix number 2
|    Mat1r       - Matrix number 1 rows
|    Mat1c       - Matrix number 1 columns
|
|  OutPuts       :
|    Mat3        - Matrix result of Mat1 + Mat2
|                    of size mat1r x mat1c
|
|  Locals        :
|    Row         - Row Index
|    Col         - Column Index
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE MATSUB             ( Mat1,Mat2                             : Matrix;
                               Mat1r,Mat1c                           : INTEGER ;
                               VAR Mat3                              : Matrix     );
   VAR
     Row,Col  : INTEGER;
   BEGIN
     INITMATRIX( Mat1r,Mat1c,1,1, Mat3 );

     FOR Row:= 1 to Mat1r DO

         FOR Col:= 1 to Mat1c DO

             ASSIGNVAL( Mat3,Row,Col, GETVAL(Mat1,Row,Col) - GETVAL(Mat2,Row,Col) );

   END;  { PROCEDURE MATSUB }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MATTRANS
|
|  This PROCEDURE finds the transpose of a matrix.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat1        - Matrix number 1
|    Mat1r       - Matrix number 1 rows
|    Mat1c       - Matrix number 1 columns
|
|  OutPuts       :
|    Mat2        - Matrix result of transpose Mat2
|
|  Locals        :
|    Row         - Row Index
|    Col         - Column Index
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE MATTRANS           ( Mat1                                  : Matrix;
                               Mat1r,Mat1c                           : INTEGER ;
                               VAR Mat2                              : Matrix );
   VAR
     Row,Col : INTEGER;
   BEGIN
     INITMATRIX( Mat1c,Mat1r,1,1, Mat2 );
     FOR Row :=1 to Mat1r DO
         FOR Col := 1 to Mat1c DO
             ASSIGNVAL( Mat2,Col,Row, GETVAL(Mat1,Row,Col) );
   END;  { PROCEDURE MATTRANS }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MAKEMAT
|
|  This PROCEDURE forms a rotation matrix for a given axis of rotation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Angl        - ANGLE through which to rotate
|    Numbr       - Number of axis for rotation
|
|  OutPuts       :
|    Matr        - Matrix of rotation
|
|  Locals        :
|    None.
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE MAKEMAT            ( Angl                                  : EXTENDED;
                               Numbr                                 : INTEGER;
                               VAR Matr                              : Matrix );
   BEGIN
     Matr:= NIL;
     INITMATRIX( 3,3,1,1, Matr);
     CASE Numbr OF
       1 : BEGIN
             ASSIGNVAL( Matr,1,1,  1.0);
             ASSIGNVAL( Matr,2,2,  COS(Angl));
             ASSIGNVAL( Matr,2,3,  SIN(Angl));
             ASSIGNVAL( Matr,3,2, -SIN(Angl));
             ASSIGNVAL( Matr,3,3,  COS(Angl));
           END;
       2 : BEGIN
             ASSIGNVAL( Matr,2,2,  1.0);
             ASSIGNVAL( Matr,1,1,  COS(Angl));
             ASSIGNVAL( Matr,1,3, -SIN(Angl));
             ASSIGNVAL( Matr,3,1,  SIN(Angl));
             ASSIGNVAL( Matr,3,3,  COS(Angl));
           END;
       3 : BEGIN
             ASSIGNVAL( Matr,1,1,  COS(Angl));
             ASSIGNVAL( Matr,1,2,  SIN(Angl));
             ASSIGNVAL( Matr,2,1, -SIN(Angl));
             ASSIGNVAL( Matr,2,2,  COS(Angl));
             ASSIGNVAL( Matr,3,3,  1.0);
           END;
     END;  { Case }
   END;  { PROCEDURE MAKEMAT }

{ ---------- Multiply a matrix and a vector ----------- }

PROCEDURE MatVecMult         ( Mat                                   : Matrix;
                               Vec                                   : Vector;
                               Mat1r,Mat1c                           : Integer ;
                               VAR VecOut                            : Vector );
   VAR
       Row,Col,ktr : Integer;
   BEGIN

     FOR Row:=1 to mat1r DO
       BEGIN
         VecOut[Row]:= 0.0;
         FOR ktr:= 1 to mat1c DO
             VecOut[Row]:= VecOut[Row] + GetVal(Mat,Row,ktr)*Vec[ktr];
       END; { For }

     MAG( VecOut );
   END;  { Procedure MatVecMult }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LUDECOMP
|
|  This PROCEDURE decomposes a matrix into an LU form.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Order       - Order of matrix
|
|  OutPuts       :
|    LU          - LU decomposition matrix
|    Index       - Index vector for pivoting
|
|  Locals        :
|    i           - Index
|    j           - Index
|    k           - Index
|    IMax        - Pivot row pointer
|    Scale       - Scale FACTOR vector
|    Sum         - Temporary Variables
|    AMax        - Temporary Variables
|    Dum         - Temporary Variables
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|
|  References    :
|    Numerical Recipes - Flannery
|
 ----------------------------------------------------------------------------- }

PROCEDURE LUDeComp           ( VAR LU                                : Matrix;
                               VAR Index                             : Intarray;
                               Order                                 : INTEGER  );
   CONST
     Small : EXTENDED = 0.000001;
   Var
     I, J, K, IMax  : INTEGER;
     Scale          : Matrix;
     Sum, AMax, Dum : EXTENDED;
   BEGIN
     Scale:= NIL;
     INITMATRIX( Order,1,1,1, Scale );
     IMax := 0;
     FOR I := 1 to Order do
       BEGIN
         AMax := 0.0;
         FOR J := 1 to Order do
             IF (ABS(GETVAL( LU,i,j)) > AMax) THEN
                 AMax := ABS(GETVAL( LU,i,j));
         IF ABS(AMax) < Small THEN
           BEGIN
             Write(' Singular Matrix AMax ');
             halt;
           END;
         ASSIGNVAL( Scale,i,1, 1.0 / AMax );
       END;
     FOR j := 1 to Order DO
       BEGIN
         FOR i := 1 to j - 1 DO
           BEGIN
             Sum := GETVAL( LU,i,j);
             FOR k := 1 to i - 1 DO
                 Sum := Sum - GETVAL( LU,i,k)*GETVAL( LU,k,j);
             ASSIGNVAL( LU,i,j, Sum );
           END;
         AMax := 0.0;
         FOR i := j to Order DO
           BEGIN
             Sum := GETVAL( LU,i,j);
             FOR k := 1 to j - 1 DO
                 Sum := Sum - GETVAL( LU,i,k)*GETVAL( LU,k,j);
             ASSIGNVAL( LU,i,j,  Sum );
             Dum := GETVAL( Scale,i,1 )*ABS(Sum);
             IF (Dum >= AMax) THEN
               BEGIN
                 IMax := i;
                 AMax := Dum;
               END;
           END;
         IF (j <> iMax) THEN
           BEGIN
             FOR k := 1 to Order do
               BEGIN
                 Dum := GETVAL( LU,iMax,K);
                 ASSIGNVAL( LU,iMax,k, GETVAL( LU,j,k ) );
                 ASSIGNVAL( LU,j,k,   Dum );
               END;
             ASSIGNVAL( Scale,iMax,1, GETVAL( Scale,j,1) );
           END;
         Index[j] := IMax;
         IF ABS(GETVAL( LU,j,j)) < Small THEN
           BEGIN
             Write(' Matrix is Singular LU ');
             halt;
           END;
         IF (j <> Order) THEN
           BEGIN
             Dum := 1.0 / GETVAL( LU,j,j);
             FOR i := j + 1 to Order DO
                 ASSIGNVAL( LU,i,j,  Dum*GETVAL( LU,i,j) );
           END;
       END;
     DELMATRIX( Scale );
   END; { PROCEDURE LuDeCmp }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LUBkSUB
|
|  This PROCEDURE finds the inverse of a matrix using LU decomposition.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Order       - Order of matrix
|    LU          - LU decomposition matrix
|    Index       - Index vector for pivoting
|
|  OutPuts       :
|    B           - Solution Vector
|
|  Locals        :
|    i           - Index
|    j           - Index
|    I0          - Pointer to non-zero element
|    IPtr        - Pivot Rwo Pointer
|    Sum         - Temporary Variables
|
|  Coupling      :
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|
|  References    :
|    Numerical Recipes - Flannery
|
 ----------------------------------------------------------------------------- }

PROCEDURE LUBkSub            ( LU                                    : Matrix;
                               Index                                 : Intarray;
                               Order                                 : INTEGER;
                               Var B                                 : Matrix );
   VAR
     I, J, IPtr, I0: INTEGER;
     Sum       : EXTENDED;
   BEGIN
     I0 := 0;
     FOR i := 1 to Order DO
       BEGIN
         IPtr := Index[i];
         Sum := GETVAL( B,IPtr,1);
         ASSIGNVAL( B,Iptr,1, GETVAL( B,i,1) );
         IF (I0 <> 0) THEN
             FOR j := I0 to i - 1 do
                 Sum := Sum - GETVAL( LU,i,j)*GETVAL( B,j,1)
           ELSE
             IF (Sum <> 0.0) THEN
                I0 := I;
         ASSIGNVAL( B,i,1,  Sum );
       END;

     ASSIGNVAL( B,Order,1, GETVAL( B,Order,1)/GETVAL( LU,Order,Order) );

     FOR i := (Order - 1) Downto 1 DO
       BEGIN
         Sum := GETVAL( B,i,1);
         FOR j := i + 1 to Order do
            Sum := Sum - GETVAL( LU,i,j)*GETVAL( B,j,1);
         ASSIGNVAL( B,i,1, Sum / GETVAL( LU,i,i) );
       END;
  END;
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MATINVERSE
|
|  This PROCEDURE finds the inverse of a matrix using LU decomposition.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat         - Matrix to invert
|    Order       - Order of matrix
|
|  OutPuts       :
|    MatInv      - Inverted matrix
|
|  Locals        :
|    i           - Index
|    j           - Index
|    Index       - Index vector for pivoting
|    LU          - LU decomposition matrix
|    B           - Operational vector to form MatInv
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|    LUDeComp    -
|    LUBkSub     -
|
|  References    :
|    Numerical Recipes - Flannery
|
 ----------------------------------------------------------------------------- }

PROCEDURE MATINVERSE         ( Mat                                   : Matrix;
                               Order                                 : INTEGER;
                               VAR MatInv                            : Matrix );
   VAR
     I, J        : INTEGER;
     Index       : Intarray;
     LU,B        : Matrix;
   BEGIN
      LU:= NIL;
      B:= NIL;
      INITMATRIX( Order,Order,1,1, LU   );
      INITMATRIX( Order,1,1,1,     B    );
      INITMATRIX( Order,Order,1,1, MatInv );

      FOR i := 1 to Order DO
        BEGIN
          Index[i] := i;
          FOR j := 1 to Order DO
              ASSIGNVAL( LU,i,j, GETVAL( Mat,i,j) );
        END;

      LUDeComp(LU, Index, Order);

      FOR j := 1 to Order DO
        BEGIN
          FOR i := 1 to Order DO
             IF (i = j) THEN
                 ASSIGNVAL( B,i,1, 1.0 )
               ELSE
                 ASSIGNVAL( B,i,1, 0.0 );

          LUBkSub(LU, Index, Order, B);

          FOR i := 1 to Order do
             ASSIGNVAL( MatInv,i,j,  GETVAL( B,i,1) );
        END;
     DELMATRIX( LU );
     DELMATRIX( B );
   END; { PROCEDURE MATINVERSE }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PRINTMAT
|
|  This PROCEDURE prints a matrix.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Mat1        - Matrix to print out
|    DecNum      - Number of decimals to write
|
|  OutPuts       :
|    None.
|
|  Locals        :
|    Row         - Row Index
|    Col         - Column Index
|
|  Coupling      :
|    GETVAL      - Gets a value from the matrix
|
 ----------------------------------------------------------------------------- }

PROCEDURE PRINTMAT           ( Mat1                                  : Matrix;
                               DecNum                                : INTEGER;
                               Title                                 : STR64 );
   VAR
       Row,Col : INTEGER;
   BEGIN
     IF DecNum > 11 THEN
         DecNum:= 10;
     Writeln( Title );
     FOR Row:= 1 to Mat1^.NumRows DO
       BEGIN
         FOR Col:= 1 to Mat1^.NumCols DO
           BEGIN
             Write( ' ',GETVAL( Mat1,Row,Col):12:DecNum );
             IF (Col MOD 6 = 0) and (Mat1^.NumCols > 6) THEN
               BEGIN
                 WriteLn;
                 Write( ' ' );
               END;
           END;
         WriteLn;
       END;
   END;  { PROCEDURE PRINTMAT }

PROCEDURE FILEPRINTMAT        ( Mat1                                 : Matrix;
                                Title                                : STR64;
                                Conv                                 : EXTENDED;
                                NumRows                              : INTEGER;
                                VAR OutFile                          : TEXT );
   VAR
       i,j       : INTEGER;
   BEGIN
     WriteLn( OutFile );
     Writeln( OutFile,Title );
     FOR i:= 1 to NumRows DO
       BEGIN
         FOR j:= 1 to Mat1^.NumCols DO
           BEGIN
             Write( OutFile,' ',GETVAL( Mat1,i,j)*Conv:16:12 );
             IF (j MOD 6 = 0) and (Mat1^.NumCols > 6) THEN
               BEGIN
                 WriteLn( OutFile );
                 Write( OutFile,' ' );
               END;
           END;
         WriteLn( OutFile );
       END;
   END;  { PROCEDURE FILEPRINTMAT }


{ ----- Print a matrix using exponential formats ------- }

PROCEDURE FILEEXPPRINTMAT       ( Mat1                     : Matrix;
                                  Title                    : STR64;
                                  Conv                     : EXTENDED;
                                  NumRows                  : INTEGER;
                                  VAR OutFile              : TEXT );
   VAR
       i,j       : INTEGER;
   BEGIN
     WriteLn( OutFile );
     Writeln( OutFile,Title );
     FOR i:= 1 to NumRows DO
       BEGIN
         FOR j:= 1 to Mat1^.NumCols DO
           BEGIN
             Write( OutFile,' ',GETVAL( Mat1,i,j)*Conv:14 );
             IF (j MOD 6 = 0) and (Mat1^.NumCols > 6) THEN
               BEGIN
                 WriteLn( OutFile );
                 Write( OutFile,' ' );
               END;
           END;
         WriteLn( OutFile );
       END;
   END;  { PROCEDURE FILEEXPPRINTMAT }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION DETERMINANT
|
|  This FUNCTION calculates the determinant value using L-U decompisition.
|  The formula must have a NON-ZERO number in the 1,1 position.  IF the
|  FUNCTION senses a NON-ZERO number in row 1, it exchanges row1 for a row
|  WITH a NON-ZERO number.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Order       - Order of determinaent (# of rows)
|    Mat1        - Matrix to find determinant of
|
|  OutPuts       :
|    Determinant - Result
|
|  Locals        :
|    i           - Index
|    j           - Index
|    k           - Index
|    n           - Index
|    Temp        -
|    D           -
|    Sum         -
|    L           -
|    U           -
|    Small       - Tolarance for comparing to 0.0
|
|  Coupling      :
|    INITMATRIX  - Initializes a matrix and sets to 0.0's
|    ASSIGNVAL   - Assigns a value to a matrix
|    GETVAL      - Gets a value from the matrix
|    DELMATRIX   - Delete a matrix
|
|  References    :
|    Marion        pg. 168-172, 126-127
|
 ----------------------------------------------------------------------------- }

FUNCTION DETERMINANT         ( Mat1                                  : Matrix;
                               Order                                 : INTEGER ) : EXTENDED;
   CONST
     Small : EXTENDED = 0.00000001;
   VAR
     i,j,k,n      : INTEGER;
     Temp, D, Sum : EXTENDED;
     L, U         : Matrix;
   BEGIN
     L:= NIL;  U:= NIL;
     Sum:= 0.0;
     { ----------- Switch a non zero row to the first row ---------- }
     IF ABS( GETVAL( Mat1,1,1 ) ) < Small THEN
       BEGIN
         j:= 1;
         WHILE j <= Order DO
           BEGIN
             IF ABS( GETVAL( Mat1,j,1 ) ) > Small THEN
               BEGIN
                 FOR k:= 1 to Order DO
                   BEGIN
                     Temp:=  GETVAL( Mat1,1,k );
                     ASSIGNVAL( Mat1,1,k, GETVAL( Mat1,j,k ) );
                     ASSIGNVAL( Mat1,j,k, Temp );
                   END;
                 j:= Order + 1;
               END;
             j:= j+1;
           END;
       END; { IF ABS(Mat1[1,1]) < Small }

     INITMATRIX( Order,Order,1,1, L );
     INITMATRIX( Order,Order,1,1, U );
     FOR i:= 1 to Order DO
         ASSIGNVAL( L,i,1, GETVAL( Mat1,i,1 ) );
     FOR j:= 1 to Order DO
         ASSIGNVAL( U,1,j, GETVAL( Mat1,1,j ) / GETVAL( L,1,1 ) );
     FOR j:= 2 to Order DO
       BEGIN
         FOR i:= j to Order DO
           BEGIN
             Sum:= 0.0;
             FOR k:= 1 to j-1 DO
                 Sum:= Sum+ GETVAL( L,i,k )* GETVAL( U,k,j );
             ASSIGNVAL( L,i,j, GETVAL( Mat1,i,j )- Sum );
           END;  { for i }
         ASSIGNVAL( U,j,j,  1.0 );
         IF j <> Order THEN
           BEGIN
             FOR i:= j+1 to Order DO
               BEGIN
                 Sum:= 0.0;
                 FOR k:= 1 to j-1 DO
                     Sum:= Sum +  GETVAL( L,j,k)* GETVAL( U,k,i );
                 ASSIGNVAL( U,j,i,( GETVAL( Mat1,j,i)-Sum)/ GETVAL( L,j,j ) );
               END;  { for i }
           END;  { IF j }
       END;  { for j }
     D:= 1.0;
     FOR i:= 1 to Order DO
         D:= D* GETVAL( L,i,i);
     Determinant:= D;
     DELMATRIX( l );
     DELMATRIX( u );
   END;  { FUNCTION Determinant }

BEGIN

END.  { Unit AstMath }

