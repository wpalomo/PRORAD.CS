
// Recuento de material por albarán de entrada.
// Entrar cantidad recontada en albarán de entrada.

// Recibe:
//		- Objeto PRORAD.

// Utiliza:
//		- Método PRORAD.recinicializar(), inicializar recuento de material por albarán.
//		- Método PRORAD.recvalidaralbaran(NumAlb), validar el albarán.
//		- Método PRORAD.recvalidarean(CodEAN), validar el EAN.
//		- Método PRORAD.recvalidarcnt(Cantid), validar la cantidad.
//		- Método PRORAD.recactualizar(), actualizar recuento.
//		- Método PRORAD.recponcero(), poner a cero cantidad recuento de la línea.
//		- Método PRORAD.recfinalizar(), cerrar recuento de material por albarán.

//		- ConsultaAlbaranes.CPP, consulta y selección de albaranes.

// Llamado desde:
//		- Prorad.CPP

// Historial de modificaciones:
//	- Parametrizar posición líneas input. Variable global _LINEA_INPUTS. AVC - 18.12.2006

#include <stdafx.h>
#include "PRORAD.h"
#include "ConsultaAlbaranes.h"

#define _LECT_ALBARAN            1
#define _VALIDAR_ALBARAN         2
#define _LECT_CODEAN             3
#define _LECT_CANTID             4
#define _MOSTRAR_ALBARAN         5
#define _DATOS_OK                6
#define _ACTUALIZAR              7

#define _INICIALIZAR            21
#define _FINALIZAR	            22

#define _CONS_ALBARANES         31
#define _PONER_CERO		        32

#define _PROGRAMA	"RECUENTO.CPP"
#define _SECCION_1  "01"

// Displays en pantalla.
void Pantalla_RecuentoMaterial(compradio::IproradPtr Iptr,	CString cstSeccion)
{
	extern int _LINEA_INPUTS;

	clrscr();

	// Visualizar campos parametrizados
	DisplayUserScr(Iptr, _PROGRAMA, cstSeccion);
	clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0);
}

// Proceso de recuento de material.
void RecuentoMaterial(compradio::IproradPtr Iptr)
{
	CString cstCodPro;
	CString cstCodArt;
	CString cstDescri;
	CString cstCodEAN;
	CString cstCanRec;
	CString cstCantid;
	CString cstAlbaran;

	CString cstLectura;

	VARIANT VariableVariant;

    _variant_t vRetVal;

	_variant_t * vCodPro  = new _variant_t;
	_variant_t * vCodArt  = new _variant_t;
	_variant_t * vDescri  = new _variant_t;
	_variant_t * vCodEAN  = new _variant_t;
	_variant_t * vCanRec  = new _variant_t;
	_variant_t * vCantid  = new _variant_t;
	_variant_t * vAlbaran = new _variant_t;

	int ret, opc;
	char * cRetVal;
	_bstr_t szRetVal, szTipoUbi;

	extern int _LINEA_INPUTS;

	opc = _INICIALIZAR;

	do
	{
		switch(opc)
		{
		case _INICIALIZAR:
			cstCodPro  = "";
			cstCodArt  = "";
			cstCodEAN  = "";
			cstCantid  = "";
			cstAlbaran = "";

			// Inicializar estado recuento de material por albarán
			vRetVal = Iptr->recinicializar();
			szRetVal = vRetVal;
			cRetVal = szRetVal;

			switch(cRetVal[0])
			{
			// OK
			case 'S':
				opc =_LECT_ALBARAN;
				break;

			// Resto de casos: Tomar como error
			default:
				// NOK
				opc =_FINALIZAR;
				break;
			}
			break;

		// Pedir nº de albarán a recontar
		case _LECT_ALBARAN:
			cstAlbaran = "";

			Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

			clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf("Introduzca albaran");
			clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("<F2:Consulta>");
			clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); ret = leer_char(&cstLectura, 10);

			cstAlbaran = cstLectura;
			vAlbaran->SetString((const char *)cstAlbaran);

			switch (ret)
			{
			// Abandonar programa
			case _TECLA_ESC:
				opc =_FINALIZAR;
				break;

			// Consulta de albaranes
			case _TECLA_F2:
				opc =_CONS_ALBARANES;
				break;

			case _TECLA_0:
				opc = _VALIDAR_ALBARAN;
				break;

			// Resto de teclas: ignorar.
			default:
				opc =_LECT_ALBARAN;
				break;
			}
			break;

		// Validar el nº de albarán
		// Se viene desde _LECT_ALBARAN (TECLA_0)  y _CONS_ALBARAN
		case _VALIDAR_ALBARAN:
			vRetVal = Iptr->recvalidaralbaran(vAlbaran);

			vRetVal.ChangeType(VT_BSTR);

			szRetVal = vRetVal;
			cRetVal = szRetVal;

			switch(cRetVal[0])
			{
			// OK
			case 'S':
				opc =_LECT_CODEAN;
				break;

			// NOK
			case 'N':
				UserError(Iptr);
				opc =_LECT_ALBARAN;
				break;

			// Error en BBDD.
			case 'C':
				UserError(Iptr);
				opc =_FINALIZAR;
				break;

			// Resto de casos: Tomar como OK
			default:
				// Albarán OK
				opc =_LECT_CODEAN;
				break;
			}
			break;

		// Solicitamos código de barras del artículo.
		case _LECT_CODEAN:
			cstCodPro = "";
			cstCodArt = "";
			cstCodEAN = "";
			cstDescri = "";
			cstCantid = "";

			Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

			clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf("Introduzca EAN");
			clreol(0, _LINEA_INPUTS + 1);
			clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); ret=leer_char(&cstLectura, 20);

			cstCodEAN = cstLectura;

			switch (ret)
			{
			// Ir a pedir albarán.
			case _TECLA_ESC:
				opc =_LECT_ALBARAN;
				break;

			case _TECLA_0:
				vCodEAN->SetString((const char *)cstCodEAN);
				vRetVal = Iptr->recvalidarean(vCodEAN);

				vRetVal.ChangeType(VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// OK
				case 'S':
					// Busca propietario
					Iptr->get_codpro(vCodPro);
					vCodPro->ChangeType(VT_BSTR);
					VariableVariant=vCodPro->Detach();
					cstCodPro=VariableVariant.bstrVal;

					// Busca artículo
					Iptr->get_codart(vCodArt);
					vCodArt->ChangeType(VT_BSTR);
					VariableVariant=vCodArt->Detach();
					cstCodArt=VariableVariant.bstrVal;

					// Busca descripción
					Iptr->get_desart(vDescri);
					vDescri->ChangeType(VT_BSTR);
					VariableVariant=vDescri->Detach();
					cstDescri=VariableVariant.bstrVal;

					// Busca cantidad recontada
					Iptr->get_canrec(vCanRec);
					vCanRec->ChangeType(VT_BSTR);
					VariableVariant=vCanRec->Detach();
					cstCanRec = VariableVariant.bstrVal;

					opc =_LECT_CANTID;
					break;

				// NOK
				case 'N':
					UserError(Iptr);
					opc =_LECT_CODEAN;
					break;

				// Error en BBDD.
				case 'C':
					UserError(Iptr);
					opc =_FINALIZAR;
					break;

				// Resto de casos: Tomar como OK
				default:
					// Albarán OK
					opc =_LECT_CANTID;
					break;
				}
				break;

			// Resto de teclas: ignorar.
			default:
				opc =_LECT_CODEAN;
				break;
			}
			break;

		// Entrar la cantidad recontada.
		case _LECT_CANTID:
			cstCantid    = "";

			Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

			clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf("Introduzca cantidad");
			clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("<F2:Poner a cero>");
			clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); ret=leer_char(&cstLectura, 10);

     		// Tratamiento lectura
            switch(ret)
            {
			// Ir a pedir EAN
			case _TECLA_ESC:
				opc =_LECT_CODEAN;
				break;

			// Poner a cero cantidad recontada de la línea
			case _TECLA_F2:
				opc =_PONER_CERO;
				break;

			case _TECLA_0:
				if (cstLectura == "") cstLectura = '0';
				cstCantid = cstLectura;
				cstCantid.Format("%06s", cstCantid);

				vCantid->SetString((const char *)cstCantid);
				vRetVal = Iptr->recvalidarcnt(vCantid);

				vRetVal.ChangeType(VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// OK
				case 'S':
					// Albarán OK
					opc =_DATOS_OK;
					break;

				// NOK
				case 'N':
					UserError(Iptr);
					opc =_LECT_CANTID;
					break;

				// Cantidad > cantidad pedida
				case 'P':
					UserError(Iptr);

					clreol(0, _LINEA_INPUTS + 1);
					ret=("Aceptar cantidad (S/N)", _LINEA_INPUTS + 1);

					switch (ret)
					{
					// Aceptar cantidad recontada > cantidad pedida
					case _TECLA_SI:
						opc = _DATOS_OK;
						break;

					// Por defecto NO acepta la cantidad
					default:
						opc = _LECT_CANTID;
						break;
					}

					break;

				// Error en BBDD
				case 'C':
					UserError(Iptr);
					opc =_FINALIZAR;
					break;

				// Resto de casos: Tomar como OK
				default:
					// Albarán OK
					opc =_DATOS_OK;
					break;
				}
				break;

			// Resto de teclas: Ignorar.
			default:
				opc =_LECT_CANTID;
				break;
			}
			break;

		// Actualizar cantidad recontada en la línea del albarán
		case _DATOS_OK:
			Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

			clreol(0, _LINEA_INPUTS + 1);
			ret=preguntar("Datos OK (S/N)", _LINEA_INPUTS + 1);

			switch (ret)
			{
			// Grabar datos
			case _TECLA_SI:
				opc = _ACTUALIZAR;
				break;

			// Resto teclas: Ignorar
			default:
				opc = _LECT_CANTID;
				break;
			}
			break;
			
		// Actualizar cantidad y estado albarán de entrada
		case _ACTUALIZAR:
			vRetVal = Iptr->recactualizar();

			vRetVal.ChangeType(VT_BSTR);

			szRetVal = vRetVal;
			cRetVal = szRetVal;

			switch(cRetVal[0])
			{
			// OK
			case 'S':
				// Recuento actualizado
				opc =_LECT_CODEAN;
				break;

			// NOK
			case 'N':
				UserError(Iptr);
				opc =_LECT_CANTID;
				break;

			// Error en BBDD
			case 'C':
				UserError(Iptr);
				opc =_FINALIZAR;
				break;

			// Resto de casos: Tomar como OK
			default:
				// Recuento actualizado
				opc =_LECT_CODEAN;
				break;
			}
			break;

		// Consulta de albaranes
		case _CONS_ALBARANES:
			ConsultaAlbaranes(Iptr);

			// Recuperar albarán actual, si hay
			Iptr->get_cnsnumalb(vAlbaran);
			vAlbaran->ChangeType(VT_BSTR);
			VariableVariant = vAlbaran->Detach();
			cstAlbaran = VariableVariant.bstrVal;
			vAlbaran->SetString((const char *)cstAlbaran);

			opc = _VALIDAR_ALBARAN;
			break;

		// Poner a cero la cantidad recuento de la línea
		case _PONER_CERO:
			clreol(0, _LINEA_INPUTS + 1);
			ret=preguntar("Poner a cero (S/N)", _LINEA_INPUTS + 1);

			switch (ret)
			{
			// Poner a cero la cantidad
			case _TECLA_SI:
				vRetVal = Iptr->recponcero();

				vRetVal.ChangeType(VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// OK
				case 'S':
					// Ir a pedir cantidad
					opc =_LECT_CODEAN;
					break;

				// Error en BBDD
				case 'C':
					UserError(Iptr);
					opc =_FINALIZAR;
					break;

				// Resto de casos: Tomar como OK
				default:
					// Pedir cantidad
					opc =_LECT_CODEAN;
					break;
				}
				break;

			// Cualquier otra respuesta, volver a pedir cantidad
			default:
				opc = _LECT_CANTID;
				break;
			}

			break;

		// Cerrar estado recuento de material
		// La otra alternativa es situar este trozo de código después de
		// while (opc != _NO_OPERACION), como en ConsultaAlbaran.CPP
		case _FINALIZAR:
			vRetVal = Iptr->recfinalizar();
			opc =_NO_OPERACION;
			break;
		}
	}
	while (opc != _NO_OPERACION);
}
