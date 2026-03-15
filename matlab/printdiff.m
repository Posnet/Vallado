% ----------------------------------------------------------------------------
%
%                           function printdiff
%
%  this function prints a covariance matrix difference
%
%  author        : david vallado                  719-573-2600   23 may 2003
%
%  revisions
%
%  inputs          description                    range / units
%    strin       - title
%    mat1        - 6x6 input matrix
%    mat2        - 6x6 input matrix
%
%  outputs       :
%
%  locals        :
%
%  references    :
%    none
%
%  printdiff( strin, cov1, cov2 )
% ----------------------------------------------------------------------------

function printdiff( strin, mat1, mat2 )

    small = 1e-18;
    
    fprintf(1,'diff %s \n', strin);
    fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e \n', mat1' - mat2');
    
    fprintf(1,'pctdiff %s pct over 1e-18  \n', strin);
%    fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f \n',100.0*((mat1' - mat2')/mat1'));
%    fprintf(1,'Check consistency of both approaches tmct2cl-inv(tmcl2ct) diff pct over 1e-18 \n');
%    fprintf(1,'-------- accuracy of tm comparing ct2cl and cl2ct --------- \n');
    tm1 = mat1';
    tm2 = mat2';
    for i=1:6
        for j = 1:6
            if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                diffmm(i,j) = 0.0;
            else
                diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
            end;
        end;
    end;
    fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n', diffmm );
    
    
    
    
    
