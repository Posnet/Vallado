(*     ----------------------------------------------------------------      *)
(*                                                                           *)

                               UNIT COMPLX;

(*                                                                           *)
(*  This file contains procedures to work with complex numbers.              *)
(*                                                                           *)
(*                           Companion code for                              *)
(*             Fundamentals of Astrodynamics and Applications                *)
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
(*            11 Jun 88  David Vallado                                       *)
(*                         Original Baseline from Turbo Technix May/June 88  *)
(*                                                                           *)
(*     ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    TYPE
       Complex = RECORD
                   R,I: Extended;
                 END;

  PROCEDURE AddComp            ( A,B                      : Complex;
                                 VAR C                    : Complex );

  PROCEDURE SubComp            ( A,B                      : Complex;
                                 VAR C                    : Complex );

  PROCEDURE MultComp           ( A,B                      : Complex;
                                 VAR C                    : Complex );

  PROCEDURE DivComp            ( A,B                      : Complex;
                                 VAR C                    : Complex );

  PROCEDURE DivRbyC            ( R                        : Extended;
                                 C                        : Complex;
                                 VAR COut                 : Complex );

  PROCEDURE MultRC             ( R                        : Extended;
                                 C                        : Complex;
                                 VAR COut                 : Complex );

  PROCEDURE SubCFromR          ( R                        : Extended;
                                 C                        : Complex;
                                 VAR COut                 : Complex );

  PROCEDURE DivCByR            ( C                        : Complex;
                                 R                        : Extended;
                                 VAR COut                 : Complex );

  Function  CompMAG            ( C                        : Complex ): Extended;

  Function  CompSQRT           ( R                        : Extended ): Extended;



(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)
{}
{ -----------------------------------------------------------------------
|
|                           PROCEDURE ADDCOMP
|
|  This PROCEDURE adds two complex numbers.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    A           - first complex number
|    B           - second complex number
|
|  OutPuts       :
|    C           - resulting complex number
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

PROCEDURE AddComp              ( A,B                      : Complex;
                                 VAR C                    : Complex );
   BEGIN
     C.R:= A.R + B.R;
     C.I:= A.I + B.I;
   END;  { PROCEDURE AddComp }

{ -----------------------------------------------------------------------
|
|                           PROCEDURE SUBCOMP
|
|  This PROCEDURE subtracts two complex numbers.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    A           - first complex number
|    B           - second complex number
|
|  OutPuts       :
|    C           - resulting complex number
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

PROCEDURE SubComp              ( A,B                      : Complex;
                                 VAR C                    : Complex );
   BEGIN
     C.R:= A.R - B.R;
     C.I:= A.I - B.I;
   END;  { PROCEDURE SubComp }

{ -----------------------------------------------------------------------
|
|                           PROCEDURE MULTCOMP
|
|  This PROCEDURE multiplies two complex numbers.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    A           - first complex number
|    B           - second complex number
|
|  OutPuts       :
|    C           - resulting complex number
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

PROCEDURE MultComp             ( A,B                      : Complex;
                                 VAR C                    : Complex );
   BEGIN
     C.R:= A.R * B.R - A.I * B.I;
     C.I:= A.I * B.I + A.I * B.R;
   END;  { PROCEDURE MultComp }

{ -----------------------------------------------------------------------
|
|                           PROCEDURE DIVCOMP
|
|  This PROCEDURE divides two complex numbers.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    A           - first complex number
|    B           - second complex number
|
|  OutPuts       :
|    C           - resulting complex number
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

PROCEDURE DivComp              ( A,B                      : Complex;
                                 VAR C                    : Complex );
   VAR
     D: Extended;
   BEGIN
     D:= SQR( B.R ) + SQR( B.I );
     C.R:= (A.R * B.R + A.I * B.I) / D;
     C.I:= (A.I * B.I - A.I * B.R) / D;
   END;  { PROCEDURE DivComp }
{}
{ -----------------------------------------------------------------------
|
|                           PROCEDURE DIVRBYC
|
|  This PROCEDURE divides a real value by a complex value.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    R           - Real value
|    C           - Complex number
|
|  OutPuts       :
|    COut        - resulting complex number
|
|  Coupling      :
|    DIVCOMP       Divides two complex numbers
|
  ------------------------------------------------------------------------ }

PROCEDURE DivRbyC              ( R                        : Extended;
                                 C                        : Complex;
                                 VAR COut                 : Complex );
   VAR
       A : Complex;
   BEGIN
     A.R:= R;
     A.I:= 0;
     DivComp( A,C,Cout );
   END;  { PROCEDURE DivRbyC }


{ -----------------------------------------------------------------------
|
|                           PROCEDURE MULTRC
|
|  This PROCEDURE multiplies a complex number by a real value.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    R           - Real value
|    C           - Complex number
|
|  OutPuts       :
|    COut        - resulting complex number
|
|  Coupling      :
|            None
|
  ------------------------------------------------------------------------ }

PROCEDURE MultRC               ( R                        : Extended;
                                 C                        : Complex;
                                 VAR COut                 : Complex );
   BEGIN
     COut.R:= C.R * R;
     COut.I:= C.I * R;
   END;  { PROCEDURE MultRc }

{ -----------------------------------------------------------------------
|
|                           PROCEDURE SUBCFROMR
|
|  This PROCEDURE subtracts a complex number from a real value.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    R           - Real value
|    C           - Complex number
|
|  OutPuts       :
|    COut        - resulting complex number
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

PROCEDURE SubCFromR            ( R                        : Extended;
                                 C                        : Complex;
                                 VAR COut                 : Complex );
   BEGIN
     COut.R:= R - C.R;
     COut.I:= -C.I;
   END;  { PROCEDURE SubCFromR }
{}
{ -----------------------------------------------------------------------
|
|                           PROCEDURE DIVCBYR
|
|  This PROCEDURE divides a complex number by a real value.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    R           - Real value
|    C           - Complex number
|
|  OutPuts       :
|    COut        - resulting complex number
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

PROCEDURE DivCByR              ( C                        : Complex;
                                 R                        : Extended;
                                 VAR COut                 : Complex );
   BEGIN
     COut.R:= C.R / R;
     COut.I:= C.I / R;
   END;  { PROCEDURE DivCByR }

{ -----------------------------------------------------------------------
|
|                           FUNCTION COMPMAG
|
|  This function finds the magnitude of a complex number.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    C           - Complex number
|
|  OutPuts       :
|    CompMAG     - Magnitude
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

FUNCTION  CompMAG              ( C                        : Complex ): Extended;
   BEGIN
     CompMAG:= SQRT( SQR(C.R) + SQR( C.I) );
   END;  { Function CompMAG }


{ -----------------------------------------------------------------------
|
|                           FUNCTION COMPSQRT
|
|  This function finds the square root of a real number and returns the complex
|    result.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    R           - Real number, usually negative
|
|  OutPuts       :
|    CompSQRT    - Square Root (real,imag)
|
|  Coupling      :
|    None
|
  ------------------------------------------------------------------------ }

FUNCTION  CompSQRT             ( R                        : Extended ): Extended;
   BEGIN
     { ----------------------- imaginary roots --------------------- }
     IF r < 0.0 THEN
       BEGIN
         CompSqrt:= SQRT( -R );
       END
       ELSE
     { ---------------------------- Real roots --------------------- }
       BEGIN
         CompSqrt:= SQRT( R );
       END;
   END;  { Function CompSQRT }

BEGIN

END.  { Unit Complx }

