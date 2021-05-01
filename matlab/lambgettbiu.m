%
% form the minimum time and universal variable matrix
%
% [tbiu, tbilu] = lambgettbiu(r1,r2,3);

function [tbi, tbil] = lambgettbiu( r1, r2, order )

    tbi = zeros(order, 2);
    %tbi = [0 0; 0 0; 0 0; 0 0; 0 0];
    for i = 1: order
        [psib, tof] = lambertumins( r1, r2, i, 'd' ); 
        tbi(i,1) = psib;
        tbi(i,2) = tof;
    end;
    
    tbil = zeros(order, 2);
    for i = 1: order
        [psib, tof] = lambertumins( r1, r2, i, 'r' ); 
        tbil(i,1) = psib;
        tbil(i,2) = tof;
    end;
end

