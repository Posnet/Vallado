     if (help == 'y')
     //    if (dbgfile != NULL)
     {
		 fprintf(dbgfile, "%85s\n",
                         " ------------------after initl  :---------------");
		 fprintf(dbgfile, "    inputs : \n");
		 fprintf(dbgfile, "%7s%15d%7s%15s%7s%15.9f%7s%15.9f%7s%15.9f\n",
         "satn", 0, "yr", " ", "ecco", ecco, "epoch", epoch, "inclo", inclo);
		 fprintf(dbgfile, "    in/out : \n");
		 fprintf(dbgfile, "%7s%15.9f\n", "no", no_kozai, "nounkozai", no_unkozai);
		 fprintf(dbgfile, "    outputs : \n");
		 fprintf(dbgfile,
              "%7s%15c%7s%15.9f%7s%15.9f%7s%15.9f%7s%15.9f%7s%15.9f\n",
               "method", method, "ainv", ainv, "ao", ao, "con41", con41,
               "con42", con42, "cosio", cosio);
		 fprintf(dbgfile, "%7s%15.9f\n", "cosio2", cosio2);
		 fprintf(dbgfile, "%7s%15.9f%7s%15.9f%7s%15.9f%7s%15.9f%7s%15.9f%7s%15.9f\n",
                         "einx", eccsq, "eccsq", eccsq, "omeosq", omeosq,
                         "posq", posq, "rp", rp, "rteosq", rteosq);
		 fprintf(dbgfile, "%7s%15.9f%7s%15.9f\n", "sinio", sinio, "gsto", gsto);
     }

