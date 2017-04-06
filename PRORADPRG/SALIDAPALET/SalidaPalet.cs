
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRORAD;

namespace SALIDAS
{
    public class SALIDAP : Base
    {
        private const int _CARGAR_DATOS = 1;

        private const int _LECT_UBIORI = 2;
        private const string _LECT_UBIORI_1 = "SAP-0001";
        private const string _LECT_UBIORI_2 = "SAP-1001";

        private const int _LECT_DCORI = 3;
        private const string _LECT_DCORI_1 = "SAP-0002";

        private const int _VALIDAR_UBIORI = 4;

        private const int _LECT_BULTO = 5;
        private const string _LECT_BULTO_1 = "SAP-0003";
        private const string _LECT_BULTO_2 = "SAP-1003";

        private const int _VALIDAR_BULTO = 6;

        private const int _DATOS_OK = 21;
        private const string _DATOS_OK_1 = "SAP-0011";

        private const int _ACTUALIZAR = 22;
        private const int _SALTAR_UBICACION = 23;
        private const int _VACIAR_OCUPACION = 24;

        private const int _INICIALIZAR = 31;
        private const int _FINALIZAR = 32;
        private const int _NO_OPERACION = 33;

        private const string _PROGRAMA = "SALIDAPALET.CPP";

        // Displays en pantalla.
        public void Pantalla_SalidaPalet(params object[] parametrosS)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametrosS[0];
            string cstSeccion = (string)parametrosS[1];

            clrscr();

            // Visualizar campos parametrizados.
            DisplayUserScr(Iptr, _PROGRAMA, cstSeccion);
            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0);

            return;
        }

        // Proceso de preparación por Palet.
        public void SalidaPalet(params object[] parametrosS)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametrosS[0];

            CString cstUbiOri = new CString();
            CString cstDCOri = new CString();
            CString cstPalet = new CString();

            string cstLectura = null;

            int ret = 0, opc;
            char RetVal;

            opc = _INICIALIZAR;

            try
            {
                do
                {
                    switch (opc)
                    {
                        case _INICIALIZAR:
                            cstUbiOri = "";
                            cstDCOri = "";
                            cstPalet = "";

                            // Inicializar estado Salida Palet.
                            RetVal = Iptr.SalInicializar();

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    opc = _CARGAR_DATOS;
                                    break;

                                // No OK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Tomar como error.
                                default:
                                    // NOK
                                    opc = _FINALIZAR;
                                    break;
                            }
                            break;

                        case _CARGAR_DATOS:
                            // Cargar datos palet a preparar.
                            RetVal = Iptr.SalPeticionUbicacion();

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    opc = _LECT_UBIORI;
                                    break;

                                // No OK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Tomar como error.
                                default:
                                    // NOK
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _FINALIZAR;
                                    break;
                            }
                            break;

                        // Pedir ubicación origen.
                        case _LECT_UBIORI:
                            cstUbiOri = "";
                            cstDCOri = "";
                            cstLectura = "";

                            Pantalla_SalidaPalet(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf(Iptr.GetMessage(_LECT_UBIORI_1, "Introduzca origen"));
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf(Iptr.GetMessage(_LECT_UBIORI_2, "<F1: Salta>"));
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2);

                            ret = leer_char(ref cstLectura, 16);

                            switch (ret)
                            {
                                // Abandonar programa
                                case _TECLA_ESC:
                                    opc = _FINALIZAR;
                                    break;

                                // Saltar ubicación.
                                case _TECLA_F1:
                                    opc = _SALTAR_UBICACION;
                                    break;

                                // Validar ubicación sin DC.
                                case _TECLA_0:
                                    cstUbiOri = cstLectura;
                                    opc = _VALIDAR_UBIORI;
                                    break;

                                // Resto de teclas: ignorar.
                                default:
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _LECT_UBIORI;
                                    break;
                            }
                            break;

                        // Pedir DC ubicación origen.
                        case _LECT_DCORI:
                            cstDCOri = "";
                            cstLectura = "";

                            Pantalla_SalidaPalet(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf(Iptr.GetMessage(_LECT_DCORI_1, "Introduzca DC"));
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1);
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2);

                            ret = leer_char(ref cstLectura, 2);

                            switch (ret)
                            {
                                // Pedir origen.
                                case _TECLA_ESC:
                                    opc = _LECT_UBIORI;
                                    break;

                                // Validar ubicación origen con DC.
                                case _TECLA_0:
                                    cstDCOri = cstLectura;
                                    opc = _VALIDAR_UBIORI;
                                    break;

                                // Saltar ubicación.
                                case _TECLA_F1:
                                    break;

                                // Resto de teclas: Ignorar.
                                default:
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _LECT_DCORI;
                                    break;
                            }
                            break;

                        // Validar la ubicación origen.
                        case _VALIDAR_UBIORI:
                            RetVal = System.Convert.ToChar(Iptr.SalValidarOrigen(cstUbiOri, cstDCOri));

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    opc = _LECT_UBIORI;
                                    break;

                                // NOK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _LECT_UBIORI;
                                    break;

                                // OK: Leer el DC
                                case 'D':
                                    opc = _LECT_DCORI;
                                    break;

                                // Error en BBDD.
                                case 'C':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Ignorar.
                                default:
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _LECT_UBIORI;
                                    break;
                            }
                            break;

                        // Pedir nº de palet / SSCC.
                        case _LECT_BULTO:
                            cstPalet = "";
                            cstLectura = "";

                            Pantalla_SalidaPalet(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf(Iptr.GetMessage(_LECT_BULTO_1, "Introduzca bulto"));
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf(Iptr.GetMessage(_LECT_BULTO_2, "<F1:Salta F3:Vacia>"));
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2);

                            ret = leer_char(ref cstLectura, 20);

                            switch (ret)
                            {
                                // Ir a pedir ubicación origen
                                case _TECLA_ESC:
                                    opc = _LECT_UBIORI;
                                    break;

                                // Saltar la ubicación.
                                case _TECLA_F1:
                                    opc = _SALTAR_UBICACION;
                                    break;

                                // Validar el bulto
                                case _TECLA_0:
                                    cstPalet = cstLectura;
                                    opc = _VALIDAR_BULTO;
                                    break;

                                // La ocupación está vacía.
                                case _TECLA_F3:
                                    opc = _VACIAR_OCUPACION;
                                    break;

                                // Resto de teclas: ignorar
                                default:
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _LECT_BULTO;
                                    break;
                            }
                            break;

                        // Validar el nº de palet
                        case _VALIDAR_BULTO:
                            RetVal = Iptr.SalValidarPalet(cstPalet);

                            switch (RetVal)
                            {
                                // OK: Confirmar.
                                case 'S':
                                    opc = _DATOS_OK;
                                    break;

                                // NOK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _LECT_BULTO;
                                    break;

                                // Error en BBDD.
                                case 'C':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Ignorar.
                                default:
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _LECT_BULTO;
                                    break;
                            }
                            break;

                        // Confirmar preparación del palet.
                        case _DATOS_OK:
                            Pantalla_SalidaPalet(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 1);
                            clreol(0, _LINEA_INPUTS + 2);
                            clreol(0, _LINEA_INPUTS + 3);

                            ret = preguntar(Iptr.GetMessage(_DATOS_OK_1, "Datos OK (S/N)"), _LINEA_INPUTS + 1);

                            switch (ret)
                            {
                                // Grabar datos.
                                case _TECLA_SI:
                                    opc = _ACTUALIZAR;
                                    break;

                                // Resto teclas: Ignorar
                                default:
                                    opc = _LECT_BULTO;
                                    break;
                            }
                            break;

                        // Actualizar palet preparado.
                        case _ACTUALIZAR:
                            RetVal = System.Convert.ToChar(Iptr.SalActualizar());

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    // Palet preparado.
                                    opc = _CARGAR_DATOS;
                                    break;

                                // NOK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _LECT_BULTO;
                                    break;

                                // Error en BBDD
                                case 'C':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Tomar como OK.
                                default:
                                    UserError("SAP-????", "Opción no controlada");
                                    opc = _CARGAR_DATOS;
                                    break;
                            }
                            break;

                        // Cerrar estado Salidas Palet.
                        case _FINALIZAR:
                            opc = _NO_OPERACION;
                            break;

                        default:
                            opc = _NO_OPERACION;
                            break;
                    }
                }while (opc != _NO_OPERACION);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }

            finally
            {
                RetVal = System.Convert.ToChar(Iptr.SalFinalizar());
            }

            return;
        }

        public void IProradEntry(COMPRADIO.PRORAD Iptr, params System.Object [] parametros)
        {
            SalidaPalet(Iptr, parametros);
            return;
        }
    }
}