
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRORAD;

namespace ENTRADA.CNS
{
    public class CONSULTAALBARANES : Base
    {
        private const int _LIN_PAG = 7;

        private const int _CMC_LECT_ALBARAN = 0;
        private const int _CMC_MOSTRAR_ALBARAN = 1;
        private const int _CMC_CARGAR_ALBARAN = 2;
        private const int _CMC_INIT_ALBARAN = 3;
        private const int _CMC_SELECCION = 4;
        private const int _CMC_VALIDAR = 5 ;

        private const int _NO_OPERACION = 21;

        private int nLineaDet = 0;         // Línea actual.

        public void Pantalla_ConsultaAlbaranes(string cstAlbaran,
                                               string cstFecha,
                                               string cstEstado,
                                               string cstLinea)
        {
            if (nLineaDet == 0)
            {
                // Visualizar la cabecera
                clrscr();
                gotoxy(0, 0); printf("Consulta de albaranes");

                gotoxy(0, 2); printf("NLIN");
                gotoxy(5, 2); printf("Albaran");
                gotoxy(16, 2); printf("Fecha");
                gotoxy(27, 2); printf("EST");
            }

            // Visualizar los datos
            nLineaDet += 1;

            gotoxy(0, nLineaDet + 2); printf("{0}", cstLinea);
            gotoxy(5, nLineaDet + 2); printf("{0}", cstAlbaran);
            gotoxy(16, nLineaDet + 2); printf("{0}", cstFecha);
            gotoxy(28, nLineaDet + 2); printf("{0}", cstEstado);

            return;
        }

        // Proceso de consulta de albaranes.
        public void ConsultaAlbaranes(params object [] parametrosA)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametrosA[0];

            string cstAlbaran = "";
            string cstFecEnt;
            string cstEstado;
            string cstLinea;
            string cstSkipR = "";

            string cstLectura = "";
            int ret, opc;
            char RetVal;

            opc = _CMC_INIT_ALBARAN;

            try
            {
                do
                {
                    switch (opc)
                    {
                        // Entrar nº de albarán inicial.
                        case _CMC_LECT_ALBARAN:
                            cstAlbaran = "";
                            cstFecEnt = "";
                            cstEstado = "";
                            cstLinea = "";

                            nLineaDet = 0;
                            cstLectura = "";

                            Pantalla_ConsultaAlbaranes(cstAlbaran, cstFecEnt, cstEstado, cstLinea);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf("Albaran inicial");
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("<ESC:Salir>");
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); ret = leer_char(ref cstLectura, 10);

                            cstAlbaran = cstLectura;

                            switch (ret)
                            {
                                // Abandonar el programa
                                case _TECLA_ESC:
                                    opc = _NO_OPERACION;
                                    break;

                                // Validar el albarán inicial.
                                case _TECLA_0:
                                    cstSkipR = "V";
                                    opc = _CMC_VALIDAR;
                                    break;

                                // Selección de albaranes.
                                case _TECLA_F2:
                                    RetVal = System.Convert.ToChar(Iptr.ConsultaAlbaranes(cstSkipR, cstAlbaran));

                                    switch (RetVal)
                                    {
                                        // OK
                                        case 'S':
                                            // Consulta OK
                                            nLineaDet = 0;
                                            opc = _CMC_MOSTRAR_ALBARAN;
                                            break;

                                        // NOK
                                        case 'N':
                                            // No puede cargar albaranes
                                            UserError(Iptr);
                                            opc = _CMC_LECT_ALBARAN;
                                            break;

                                        // Resto de casos: Tomar como OK
                                        default:
                                            // Albarán OK
                                            nLineaDet = 0;
                                            opc = _CMC_MOSTRAR_ALBARAN;
                                            break;
                                    }
                                    break;

                                // Resto de teclas: Ignorar
                                default:
                                    opc = _CMC_LECT_ALBARAN;
                                    break;
                            }
                            break;

                        // Mostrar la siguiente línea de albarán.
                        case _CMC_MOSTRAR_ALBARAN:
                            cstSkipR = "M";
                            RetVal = System.Convert.ToChar(Iptr.ConsultaAlbaranes(cstSkipR, cstAlbaran));

                            switch (RetVal)
                            {
                                // EOF
                                case 'O':
                                    // No hay más líneas.
                                    UserError(Iptr);
                                    opc = _CMC_LECT_ALBARAN;
                                    break;

                                // OK
                                case 'S':
                                    // Recuperar código de barras
                                    cstAlbaran = Iptr.CnsNumAlb;

                                    // Recuperar la fecha de albarán.
                                    cstFecEnt = String.Format("{0:d}", Iptr.CnsFecEnt);

                                    // Recuperar el estado del albarán.
                                    cstEstado = Iptr.CnsEstAlb;

                                    // Recuperar línea relativa.
                                    cstLinea = Iptr.CnsOrden.ToString();

                                    // Visualizar albarán actual.
                                    Pantalla_ConsultaAlbaranes(cstAlbaran, cstFecEnt, cstEstado, cstLinea);

                                    if (nLineaDet>=_LIN_PAG)
                                    {
                                        // Pantalla llena: Seleccionar albarán ó avanzar pantalla
                                        opc = _CMC_SELECCION;
                                        break;
                                    }

                                    // Cargar más albaranes.
                                    opc = _CMC_MOSTRAR_ALBARAN;
                                    break;

                                // Error
                                case 'E':
                                    // Error de programa
                                    UserError(Iptr);
                                    opc = _NO_OPERACION;
                                    break;

                                // Resto de casos: Error.
                                default:
                                    // Error
                                    clreol(0, _LINEA_INPUTS + 1);
                                    gotoxy(0, _LINEA_INPUTS + 1);
                                    printf("Resultado indefinido. Pulse tecla.");
                                    getkey();
                                    opc = _NO_OPERACION;
                                    break;
                            }
                            break;

                        // Tomar datos del albarán activo.
                        case _CMC_CARGAR_ALBARAN:
                            // Recuperar albarán actual, si hay.
                            cstAlbaran = Iptr.NumAlb;
                            cstSkipR = "V";
                            opc = _CMC_VALIDAR;
                            break;

                        // Validar el albarán.
                        // Se viene desde _CMC_CARGAR_ALBARAN y _CMC_LECT_ALBARAN(TECLA_0)
                        case _CMC_VALIDAR:
                            RetVal = System.Convert.ToChar(Iptr.ConsultaAlbaranes(cstSkipR, cstAlbaran));

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    // Albarán OK
                                    nLineaDet = 0;
                                    opc = _CMC_MOSTRAR_ALBARAN;
                                    break;

                                // NOK
                                case 'N':
                                    // No hay albaranes.
                                    opc = _CMC_LECT_ALBARAN;
                                    break;

                                // Error
                                case 'E':
                                    // Error de programa
                                    UserError(Iptr);
                                    opc = _CMC_LECT_ALBARAN;
                                    break;

                                // Resto de casos: Tomar como OK
                                default:
                                    // Albarán OK
                                    nLineaDet = 0;
                                    opc = _CMC_MOSTRAR_ALBARAN;
                                    break;
                            }
                            break;

                        // Seleccionar albarán a procesar.
                        case _CMC_SELECCION:
                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf("Seleccionar albaran");
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("<F2:Inicio F3:Sigue ESC:Salir>");
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); ret = leer_char(ref cstLectura, 10);

                            cstAlbaran = cstLectura;

                            switch (ret)
                            {
                                // Abandonar programa
                                case _TECLA_ESC:
                                    opc = _NO_OPERACION;
                                    break;

                                // Devolver albarán seleccionado
                                case _TECLA_0:
                                    cstSkipR = "S";
                                    RetVal = System.Convert.ToChar(Iptr.ConsultaAlbaranes(cstSkipR, cstAlbaran));

                                    switch (RetVal)
                                    {
                                        // OK
                                        case 'S':
                                            // Devolver albarán ya asignado a propiedad y salir
                                            opc = _NO_OPERACION;
                                            break;

                                        // NOK
                                        case 'N':
                                            // La selección no es correcta
                                            UserError(Iptr);
                                            opc = _CMC_SELECCION;
                                            break;

                                        // Error
                                        case 'E':
                                            // Error
                                            UserError(Iptr);
                                            opc = _NO_OPERACION;
                                            break;

                                        // Resto de casos: Pedir selección
                                        default:
                                            // Opción no controlada
                                            opc = _CMC_LECT_ALBARAN;
                                            break;
                                    }
                                    break;

                                // Pedir albarán inicial
                                case _TECLA_F2:
                                    opc = _CMC_LECT_ALBARAN;
                                    break;

                                // Mostrar más albaranes
                                case _TECLA_F3:
                                    nLineaDet = 0;
                                    opc = _CMC_MOSTRAR_ALBARAN;
                                    break;

                                // Resto de teclas: Ignorar
                                default:
                                    opc = _CMC_SELECCION;
                                    break;
                            }
                            break;

                        // Inicializar valores.
                        case _CMC_INIT_ALBARAN:
                            cstAlbaran = "";
                            cstFecEnt = "";
                            cstEstado = "";
                            cstLinea = "";

                            nLineaDet = 0;
                            opc = _CMC_CARGAR_ALBARAN;
                            break;

                        default:
                            opc = _NO_OPERACION;
                            break;
                    }
                } while (opc != _NO_OPERACION);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }

            finally
            {
                // Cerrar proceso de consulta.
                RetVal = Convert.ToChar(Iptr.ConsultaAlbaranes("Z", cstLectura));
            }

            return;
        }
    }
}
