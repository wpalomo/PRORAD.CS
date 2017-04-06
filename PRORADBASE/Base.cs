
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRORAD
{
    public class Base
    {
        //----------------------------------------------------
        // Constantes de uso PÚBLICO de la aplicación.
        //----------------------------------------------------
        private const int _FINALIZAR = -1;

        public const int _TECLA_0 = 0;

        public const int _TECLA_F1 = -1;
        public const int _TECLA_F2 = -2;
        public const int _TECLA_F3 = -3;
        public const int _TECLA_F4 = -4;
        public const int _TECLA_F6 = -6;

        public const int _TECLA_ESC = -12;

        public const int _TECLA_NO = -4;
        public const int _TECLA_CANCEL = -6;
        public const int _TECLA_SI = -9;

        public const char TECLA_OK = '\r';
        public const char TECLA_OK2 = ']';
        public const int TECLA_VOL = 27;
        public const char TECLA_VOL2 = '$';

        public static int _LINEA_INPUTS;
        public static int _COLUMNA_INPUTS;

        public static string _SECCION_1 = "01";

        public static void printf(object texto)
        {
            Console.Write(texto);
            return;
        }

        public static void printf(object formato, params object[] argumentos)
        {
            Console.Write(String.Format((string)formato, argumentos));
            return;
        }

        public static void clrscr()
        {
            Console.Clear();
            return;
        }

        public static void clreol(int XCoord, int YCoord)
        {
            gotoxy(XCoord, YCoord);
            Console.Write(new string(' ', Console.BufferWidth));

            return;
        }

        public static void gotoxy(int XCoord, int YCoord)
        {
            Console.SetCursorPosition(XCoord, YCoord);
            return;
        }

        public static int getkey()
        {
            int RetVal;
            var input = Console.ReadKey();

            switch (input.Key)
            {
                case ConsoleKey.F1:
                    RetVal = _TECLA_F1;
                    break;

                case ConsoleKey.F2:
                    RetVal = _TECLA_F2;
                    break;

                case ConsoleKey.F3:
                    RetVal = _TECLA_F3;
                    break;

                case ConsoleKey.F4:
                    RetVal = _TECLA_F4;
                    break;

                case ConsoleKey.F6:
                    RetVal = _TECLA_F6;
                    break;

                case ConsoleKey.Escape:
                    RetVal = _TECLA_ESC;
                    break;

                case ConsoleKey.Enter:
                    RetVal = TECLA_OK;
                    break;

                case ConsoleKey.Backspace:
                default:
                    RetVal = (int)input.Key;
                    break;
            }

            return RetVal;
        }

        //----------------------------------------------------
        // Devuelve la coordenada horizontal del cursor.
        //----------------------------------------------------
        public static int WhereX()
        {
            return Console.CursorLeft;
        }

        //----------------------------------------------------
        // Devuelve la coordenada vertical del cursor.
        //----------------------------------------------------
        public static int WhereY()
        {
            return Console.CursorTop;
        }

        //----------------------------------------------------
        // Devuelve una cadena por teclado.
        //----------------------------------------------------
        public static int leer_char(ref string leer, int POS_MAX)
        {
            int i, num_car = 0, temp;
            char[] arrai = new char[40];
            char letra = ' ';
            int x, y;

            x = WhereX();
            y = WhereY();
            for (i = 0; i < 40; i++) arrai[i] = ' ';
            leer = null;

            Console.Write(new string('_', POS_MAX));

            gotoxy(x, y);

            while (letra != TECLA_OK && letra != TECLA_VOL && letra != TECLA_VOL2 && letra != 300 && num_car < POS_MAX)
            {
                temp = getkey();
                if (temp < 0)
                    return temp;

                letra = (char)temp;
                if (letra != TECLA_OK && (Char.IsControl(letra) == false))
                {
                    if (num_car <= POS_MAX)
                    {
                        arrai[num_car] = letra;
                        num_car++;
                    }
                }
                else
                {
                    i = letra;
                }
                if (letra == '\b' && num_car > 0)
                {
                    printf("_\b_\b");
                    num_car--;
                    arrai[num_car] = ' ';
                }
            }

            for (i = 0; i < num_car; i++)
                leer += arrai[i];

            if (letra == TECLA_VOL)
                return 1;

            if (letra == TECLA_VOL2)
                return 2;

            return 0;
        }

        //----------------------------------------------------
        // Devuelve S/N/Cancel/ESC.
        //----------------------------------------------------
        public static int preguntar(string cstPregunta, int pos)
        {
            int saltar, ret, tecla;

            clreol(0, pos); clreol(0, pos + 1);
            gotoxy(0, pos); printf("{0}", cstPregunta);

            saltar = 0;
            do
            {
                gotoxy(0, pos + 1); tecla = getkey();
                if (tecla == -12)
                    saltar = 1;

                if ((tecla == 's' || tecla == 'S' ||
                     tecla == 'n' || tecla == 'N' ||
                     tecla == 'c' || tecla == 'C'))
                    saltar = 1;
            } while (saltar == 0);

            switch (tecla)
            {
                case 's':
                case 'S':
                    ret = _TECLA_SI;
                    break;

                case 'n':
                case 'N':
                    ret = _TECLA_NO;
                    break;

                case 'c':
                case 'C':
                    ret = _TECLA_CANCEL;
                    break;

                case -12:
                    ret = _TECLA_ESC;
                    break;

                default:
                    ret = _TECLA_NO;
                    break;
            }

            return ret;
        }

        //---------------------------------------------------------------------
        // Visualizar avisos y mensajes de error.
        // Historial de modificaciones:
        //---------------------------------------------------------------------
        public void UserError(string errorCode, string errorText)
        {
            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("{0,-10}", errorCode.TrimEnd());
            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); printf("{0,-30}", errorText.TrimEnd());
            getkey();

            return;
        }

        public void UserError(COMPRADIO.PRORAD Iptr)
        {
            UserError(Iptr.UsrErrorC, Iptr.UsrError);

            return;
        }

        public string GetMessage()
        {
            return "" ;
        }

        //-------------------------------------------------
        // Visualizar datos en la pantalla.
        //-------------------------------------------------
        public void DisplayUserScr(COMPRADIO.PRORAD Iptr, string cstPrograma, string cstSeccion, string cstFichero = "PANTALLAS")
        {
            char RetVal;
            int x, y;
            object cstValor, cstMask;

            for (;;)
            {
                RetVal = Iptr.ScrGenerarNextLinea(cstPrograma, cstSeccion, cstFichero);

                switch (RetVal)
                {
                    // ErrorBBDD.
                    case 'C':
                        UserError(Iptr.UsrErrorC, Iptr.UsrError);
                        return;

                    // OK: Visualizar.
                    case 'S':
                        // Obtener el valor a visualizar.
                        cstValor = Iptr.ScrValor;

                        // Obtener la máscara del valor a visualizar.
                        cstMask = Iptr.ScrMascara;

                        // Obtener la coordenada X
                        x = Iptr.XCoord;

                        // Obtener la coordenada Y
                        y = Iptr.YCoord;

                        gotoxy(x, y);
                        printf(cstMask, cstValor);
                        //printf(cstValor);
                        break;

                    // NOK
                    case 'N':
                        UserError(Iptr);
                        return;

                    // No hay mas datos.
                    case 'O':
                        return;

                    // Resto de casos: Ignorar.
                    default:
                        continue;
                }
            }
        }

    }

    public class CString
    {
        private readonly string cstX;

        public CString()
        {
            cstX = "";
        }

        public CString(string strValor)
        {
            cstX = strValor;
        }

        public static implicit operator string (CString v)
        {
            return v.cstX;
        }

        public static implicit operator CString(string a)
        {
            return new CString(a);
        }
    }
}
