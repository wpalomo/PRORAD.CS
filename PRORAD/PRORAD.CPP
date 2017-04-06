
// PRORAD.cpp : Defines the entry point for the console application.

// Version v.1.0

// Historial de modificaciones:
// 10.06.2008 (AVC)	Mostrar código de mensaje y mensaje corto.
//					Modificado método UserError

// 17.09.2014 (AVC) Agregar opciones de traslado entre ubicaciones.
//					Modificado menú general.
//					Modificado menú MOVIMIENTOS.

#include "stdafx.h"
#include <wincon.h>

#include "PRORAD.h"

// Módulos para entradas de material.
#include "Recuento.h"
#include "RecuentoPalet.h"
#include "UbicarPalet.h"

// Módulos para salidas de material.
#include "SalidaMAC.h"
#include "SalidaPalet.h"
#include "RepartoMAC.h"

// Módulos para reposiciones.
#include "Reposiciones.h"
#include "RecogidaOrigen.h"
#include "EntregaDestino.h"

// Módulos para expediciones.
#include "ConsolidarMAC.h"
#include "DocumentoMuelle.h"
#include "ConfirmarCarga.h"
#include "ConfirmarListaCarga.h"

// Módulos para movimientos entre ubicaciones.
#include "SalidaOrigen.h"
#include "EntradaDestino.h"
#include "ReubicarPalet.h"
#include "ReubicarAutomatico.h"
#include "TrasladoOrigen.h"
#include "TrasladoDestino.h"

// Módulos para inventarios.
#include "InventarioLista.h"

// Módulos para consultas.
#include "ConsultaPalet.h"
#include "ConsultaUbicacion.h"
#include "ConsultaMAC.h"
#include "Recogida_Tienda.h"
#include "Devolucion_Mostrador.h"
#include "Preparar_Documento.h"
#include "SuperMac.h"
#include "Preparacion_Tienda.h"

// Módulos auxiliares.
#include "ConsultaAlbaran.h"
#include "ConsultaAlbaranes.h"

typedef unsigned char uint8;
typedef unsigned short uint16;
typedef unsigned long uint32;
typedef signed char int8;
typedef signed short int16;
typedef signed long int32;
typedef int8 char8;
typedef uint8 uchar8;

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define TECLA_OK	'\r'
#define TECLA_OK2	']'
#define TECLA_VOL	27
#define TECLA_VOL2	'$'

#define col 80
#define fil 25

#define _INICIALIZAR            80
#define _FINALIZAR	            81

#define _LECTURA_OPERARIO		91
#define _LECTURA_TERMINAL		92
#define _VALIDAR_OPERARIO		93
#define _VALIDAR_TERMINAL		94
#define _DESCONECTAR_OPERARIO	95

#define _DESCONECTAR_BBDD		96

#define _DATOS_OK				97

#define _PROGRAMA	"PRORAD.CPP"
#define _SECCION_0  "00"					// Menú principal.
#define _SECCION_E0 "E0"					// Menú entradas.
#define _SECCION_S0 "S0"					// Menú salidas.
#define _SECCION_S1 "S1"					// Menú salidas - preparación.
#define _SECCION_S2 "S2"					// Menú salidas - reposiciones.
#define _SECCION_S3 "S3"					// Menú salidas - expedición.
#define _SECCION_I0 "I0"					// Menú inventarios.
#define _SECCION_M0 "M0"					// Menú movimientos.
#define _SECCION_C0 "C0"					// Menú consultas.

int _LINEA_INPUTS;
int _COLUMNA_INPUTS;

/////////////////////////////////////////////////////////////////////////////
// The one and only application object

CWinApp theApp;
void dump_com_error( _com_error &e );

// maximum size of the buffer to be returned from FormatMessage 
#define MAX_MSG_BUF_SIZE 512
#define TERMINAL 12

using namespace std;
HANDLE hIn,hOut;
DWORD  nLength=col*fil; // 80 COLUMNAS x 25 FILAS
DWORD  nLengthLin=col;  // 80 COLUMNAS

CString lectura_operario;

void gotoxy(unsigned uX, unsigned uY)
{
   COORD coord;

   coord.X = (WORD)(uX);
   coord.Y = (WORD)(uY);

   SetConsoleCursorPosition(hOut, coord);
}

void clrscr()
{
DWORD dwCharsWritten;
COORD coord;

	coord.X = (WORD) 0;
    coord.Y = (WORD) 0;
	FillConsoleOutputCharacter(hOut,(TCHAR) ' ',nLength,coord, &dwCharsWritten);
    gotoxy(0, 0);
}

void clreol(int posicionx, int posiciony)
{
DWORD dwCharsWritten;
COORD coord;
int longitud;

	longitud=80-posicionx;
	coord.X=posicionx;
	coord.Y=posiciony;
	FillConsoleOutputCharacter(hOut,(TCHAR) ' ',longitud,coord, &dwCharsWritten);
    gotoxy(posicionx, posiciony);
}

// Produce un retardo de wait segundos.
void sleep(clock_t wait)
{
   clock_t goal;

   wait *= CLOCKS_PER_SEC;
   goal = wait + clock();
   while( goal > clock() )
      ;
}

unsigned getkey()
{

// Lee un carácter de la consola
   INPUT_RECORD ir;
   DWORD dwRecordCount;
   unsigned uResult = 0U;
   KEY_EVENT_RECORD *pke = &ir.Event.KeyEvent;

   do
   {
      ReadConsoleInput(hIn, &ir, 1, &dwRecordCount);

      if (pke->bKeyDown==TRUE)
         if (pke->uChar.AsciiChar!=0)
            if (pke->uChar.AsciiChar>0)
			{
					if (pke->uChar.AsciiChar=='$')
						getkey();



					if (pke->wVirtualKeyCode!=187)
						uResult = pke->uChar.AsciiChar;
					if (pke->uChar.AsciiChar==VK_ESCAPE)
						return -12;

				uResult = pke->uChar.AsciiChar;
			}
            else
               uResult = (int)(pke->uChar.AsciiChar)-CHAR_MIN;
         else
            // Se está pulsando una tecla de control. Ignoramos las
            // pulsaciones de SHIFT, CTRL, ALT y xxx-LOCK.
            switch (pke->wVirtualKeyCode)
            {
			 case VK_F1:
					uResult = -1;
					break;
			 case VK_F2:
					uResult = -2;
					break;
			 case VK_F3:
					uResult = -3;
					break;
			 case VK_F4:
					uResult = -4;
					break;
			 case VK_F6:
					uResult = -6;
					break;
			 case VK_F7:
					uResult = -7;
					break;
			 case VK_F8:
					uResult = -8;
					break;
			 case VK_F9:
					uResult = -9;
					break;
			 case VK_F10:
					uResult = -10;
					break;
			 case VK_F11:
					uResult = -11;
					break;
			 case VK_F12:
					uResult = -12;
					break;			 
			 case VK_ESCAPE:
					uResult = -13;
					break;

			 case VK_SHIFT:
            case VK_CONTROL:
            case VK_MENU:
               break;

            case VK_SCROLL:
            case VK_CAPITAL:
            case VK_NUMLOCK:
               break;

            default:
               uResult = (pke->wVirtualScanCode)*(-1);
			   
		 }
   } while (uResult==0U);

   return uResult;
}

unsigned WhereX()
{
   CONSOLE_SCREEN_BUFFER_INFO csbi;

   GetConsoleScreenBufferInfo(hOut, &csbi);
   return csbi.dwCursorPosition.X;
}

// Devuelve la coordenada vertical del cursor.
unsigned WhereY()
{
   CONSOLE_SCREEN_BUFFER_INFO csbi;

   GetConsoleScreenBufferInfo(hOut, &csbi);
   return csbi.dwCursorPosition.Y;
}

int leer_num(CString *leer, int POS_MAX)
{
int i, num_car=0, temp;
char array[50];
char letra=' ';
int x,y;

	x=WhereX();
	y=WhereY();
	for (i=0;i<50;i++)	array[i]=' ';
	(*leer)="";

	for (i=x;i<x+POS_MAX;i++)
		printf("_");

	gotoxy(x,y);
	FlushConsoleInputBuffer(hIn);
	while (letra!=TECLA_OK && letra!=TECLA_VOL
		   && letra!=TECLA_VOL2 && letra!=300)
	{
		temp=getkey();
		if (temp<0)
			return temp;
		letra=temp;
		if (letra!=TECLA_OK && (isdigit(letra) || isspace(letra)))
		{
			if (num_car!=POS_MAX)
			{
				array[num_car]= letra;
				num_car++;
				printf("%c", letra);
			}
		}
		else
		{
			i=letra;
		}
		if (letra=='\b' && num_car>0)
		{
			printf("\b \b", letra);
			num_car--;
			array[num_car]=' ';
		}
	}

	for (i=0;i<num_car;i++)
		(*leer)+=array[i];

	if (letra==TECLA_VOL)
		return 1;

	if (letra==TECLA_VOL2)
		return 2;

	return 0;
}

int leer_char(CString *leer, int POS_MAX)
{
int i, num_car=0, temp;
char array[40];
char letra=' ';
int x, y;

	x=WhereX();
	y=WhereY();
	for (i=0;i<40;i++)	array[i]=' ';
	(*leer)="";

	for (i=x;i<x+POS_MAX;i++)
		printf("_");

	gotoxy(x,y);
	FlushConsoleInputBuffer(hIn);
	while (letra!=TECLA_OK && letra!=TECLA_VOL
		   && letra!=TECLA_VOL2 && letra!=300)
	{
		temp=getkey();
		if (temp<0)
			return temp;
		letra=temp;
	
		if (letra!=TECLA_OK && (isprint(letra)))
		{
			if (num_car!=POS_MAX)
			{
				array[num_car]= letra;
				num_car++;
				printf("%c", letra);
			}
		}
		else
		{
			i=letra;
		}
		if (letra=='\b' && num_car>0)
		{
			printf("\b_\b", letra);
			num_car--;
			array[num_car]=' ';
		}
	}

	for (i=0;i<num_car;i++)
		(*leer)+=array[i];

	if (letra==TECLA_VOL)
		return 1;

	if (letra==TECLA_VOL2)
		return 2;

	return 0;
}

void initconsole()
{
// Lee Handles de Entrada/Salida estándard
	hIn=GetStdHandle(STD_INPUT_HANDLE);
	hOut=GetStdHandle(STD_OUTPUT_HANDLE);
// Limpia eventos de teclado
    while (kbhit())
       getkey();
}

// Visualizar mensajes de error.
// Historial de modificaciones:
// 10.06.2008 (AVC)	Mostrar código de mensaje y texto abreviado

void UserError(compradio::IproradPtr Iptr)
{
	char str[64], strC[64], szErrBuf[64], szCErrBuf[64];
	_variant_t * UsrError = new _variant_t;
	_variant_t * UsrErrorC= new _variant_t;

	_bstr_t szUsrError, szUsrErrorC;

	Iptr->get_usrerror(UsrError);
	UsrError->ChangeType(VT_BSTR);
	Iptr->get_usrerrorc(UsrErrorC);
	UsrErrorC->ChangeType(VT_BSTR);

	szUsrError = UsrError;
	szUsrErrorC= UsrErrorC;

	strcpy((char *)szErrBuf, szUsrError);
	sprintf((char*)str, "%s", szErrBuf);
	strcpy((char *)szCErrBuf, szUsrErrorC);
	sprintf((char*)strC, "%s", szCErrBuf);

	clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("%s", strC);
	clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); printf("%s", str);

	getkey();
}

// Visualizar datos en la pantalla.
void DisplayUserScr(compradio::IproradPtr Iptr, CString cstPrograma, CString cstSeccion)
{
	char * cRetVal;
	_bstr_t szRetVal;
	_variant_t vRetVal;
	int x, y;

	VARIANT VariableVariant;
	CString cstValor, cstMask;
	
	_variant_t * vValor = new _variant_t;
	_variant_t * vMask = new _variant_t;
	_variant_t * vX = new _variant_t;
	_variant_t * vY = new _variant_t;

	_variant_t * vPrograma = new _variant_t;
	_variant_t * vSeccion = new _variant_t;

	vPrograma->SetString((const char *)cstPrograma);
	vSeccion->SetString((const char *)cstSeccion);

	for (; ;)
	{
		vRetVal = Iptr->scrgenerarnxtlinea(vPrograma, vSeccion);
		vRetVal.ChangeType(VT_BSTR);
		szRetVal = vRetVal;
		cRetVal = szRetVal;

		switch(cRetVal[0])
		{
		// OK: Visualizar
		case 'S':
			// Obtener el valor a visualizar
			Iptr->get_scrvalor(vValor);
			vValor->ChangeType(VT_BSTR);
			VariableVariant = vValor->Detach();
			cstValor = VariableVariant.bstrVal;

			// Obtener la máscara del valor a visualizar
			Iptr->get_scrmascara(vMask);
			vMask->ChangeType(VT_BSTR);
			VariableVariant = vMask->Detach();
			cstMask = VariableVariant.bstrVal;

			// Obtener la coordenada X
			Iptr->get_xcoord(vX);
			vX->ChangeType(VT_BSTR);
			szRetVal = vX;
			cRetVal = szRetVal;
			x = atoi(cRetVal);

			// Obtener la coordenada Y
			Iptr->get_ycoord(vY);
			vY->ChangeType(VT_BSTR);
			szRetVal = vY;
			cRetVal = szRetVal;
			y = atoi(cRetVal);

			gotoxy(x, y);
			printf(cstMask, cstValor);
			break;

		// NOK
		case 'N':
			UserError(Iptr);
			return;

		// No hay mas datos
		case 'O':
			return;

		// Resto de casos: Ignorar
		default:
			continue;
		}
	}
}

int display_pantalla(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	pantalla = 1;
	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_0);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla=='4' || tecla=='5' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case '4':
	case -4:
		return 4;
		break;
	case '5':
	case -5:
		return 5;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_entradas(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_E0);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_salidas(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_S0);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_preparacion(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_S1);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla=='4' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case '4':
	case -4:
		return 4;
		break;

	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_reposiciones(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_S2);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_expediciones(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_S3);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla=='4' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case '4':
	case -4:
		return 4;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_inventarios(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_I0);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_movimientos(compradio::IproradPtr Iptr, int pantalla)
{
	char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_M0);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla=='4' || tecla=='5' || tecla=='6' || tecla=='7' || tecla=='8' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;

	case '2':
	case -2:
		return 2;
		break;

	case '3':
	case -3:
		return 3;
		break;

	case '4':
	case -4:
		return 4;
		break;

	case '5':
	case -5:
		return 5;
		break;

	case '6':
	case -6:
		return 6;
		break;

	case '7':
	case -7:
		return 7;
		break;

	case '8':
	case -8:
		return 8;
		break;

	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

int display_pantalla_consultas(compradio::IproradPtr Iptr, int pantalla)
{
char tecla;

	clrscr();
	DisplayUserScr(Iptr, _PROGRAMA, _SECCION_C0);

	FlushConsoleInputBuffer(hIn);
	FlushConsoleInputBuffer(hOut);

	do{
		gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
		tecla=getkey();
	}while (!(tecla=='1' || tecla=='2' || tecla=='3' || tecla==13 || tecla==-12));

	switch(tecla)
	{
	case '1':
	case -1:
		return 1;
		break;
	case '2':
	case -2:
		return 2;
		break;
	case '3':
	case -3:
		return 3;
		break;
	case -12:
		return 0;
		break;
	case 13:
	default:
		return pantalla;
		break;
	}
}

void menu(CString oper, compradio::IproradPtr Iptr)
{
int ret, ret1, ret2, ret3;
	ret = -1;
	do
	{
		switch (ret)
		{
		// Entradas
		case 1:
			ret1 = display_pantalla_entradas(Iptr, ret);

			switch (ret1)
			{
			case 0:
				ret = -1;
				break;

			case 1:
				RecuentoMaterial(Iptr);
				break;

			case 2:
				RecuentoPalet(Iptr);
				break;

			case 3:
				UbicarPalet(Iptr);
				break;

			default:
				ret1 = display_pantalla_entradas(ret, Iptr);
				break;
			}
			break;

		case 2:
			// Salidas
			ret1 = -1; ret2 = -1;

			do
			{
				switch (ret2)
				{
				case 0:
					ret1 = 0;
					ret = -1;
					break;

				case 1:
					// Salidas - Preparación
					ret3 = display_pantalla_preparacion(Iptr, ret);

					switch(ret3)
					{
					case 0:
						ret1 = -1; ret2 = -1;
						break;

					case 1:
						SalidaPalet(Iptr);
						break;

					case 2:
						SalidaMAC(Iptr,"");
						break;

					case 3:
						Preparacion_Tienda(Iptr,"");
						break;

					case 4:
						RepartoMAC(Iptr);
						break;

					case 5:
						Preparar_Documento(Iptr);
						break;


					default:
						ret3 = display_pantalla_preparacion(Iptr, ret);
						break;
					}
					break;

				case 2:
					// Salidas - Reposiciones
					ret3 = display_pantalla_reposiciones(Iptr, ret);

					switch(ret3)
					{
					case 0:
						ret1 = -1; ret2 = -1;
						break;

					case 1:
						Reposiciones(Iptr);
						break;

					case 2:
						SalidaOrigen(Iptr);
						break;

					case 3:
						EntradaDestino(Iptr);
						break;

					default:
						ret3 = display_pantalla_reposiciones(Iptr, ret);
						break;
					}
					break;

				case 3:
					// Salidas - Expediciones
					ret3 = display_pantalla_expediciones(Iptr, ret);

					switch(ret3)
					{
					case 0:
						ret1 = -1; ret2 = -1;
						break;

					case 1:
						//ConsolidarMAC(Iptr);
						SuperMac(Iptr);
						break;

					case 2:
						DocumentoMuelle(Iptr);
						break;

					case 3:
						ConfirmarCarga(Iptr);
						break;

					case 4:
						ConfirmarListaCarga(Iptr);
						break;

					default:
						ret3 = display_pantalla_expediciones(Iptr, ret);
						break;
					}
					break;

				default:
					ret2 = display_pantalla_salidas(Iptr, ret);
					break;
				}
			} while (ret1 != 0);
			break;

		case 3:
			// Inventarios
			ret1 = display_pantalla_inventarios(Iptr, ret);

			switch (ret1)
			{
			case 0:
				ret=-1;
				break;

			case 1:
				InventarioLista(Iptr);
				break;

			default:
				ret1 = display_pantalla_inventarios(Iptr, ret);
				break;
			}
			break;

		case 4:
			// Movimientos
			ret1 = display_pantalla_movimientos(Iptr, ret);

			switch (ret1)
			{
			case 0:
				ret=-1;
				break;

			case 1:
				RecogidaOrigen(Iptr);
				break;

			case 2:
				EntregaDestino(Iptr);
				break;

			case 3:
				ReubicarPalet(Iptr);
				break;

			case 4:
				Recogida_Tienda(Iptr);
				break;

			case 5:
				Devolucion_Mostrador(Iptr);
				break;

			case 6:
				ReubicarAutomatico(Iptr);
				break;

			case 7:
				TrasladoOrigen(Iptr);
				break;

			case 8:
				TrasladoDestino(Iptr);
				break;

			default:
				ret1 = display_pantalla_movimientos(Iptr, ret);
				break;
			}
			break;

		case 5:
			// Consultas
			ret1 = display_pantalla_consultas(Iptr, ret);

			switch (ret1)
			{
			case 0:
				ret=-1;
				break;

			case 1:
				ConsultaPalet(Iptr);
				break;

			case 2:
				ConsultaUbicacion(Iptr);
				break;

			case 3:
				ConsultaMAC(Iptr);
				break;

			default:
				ret1 = display_pantalla_consultas(Iptr, ret);
				break;
			}
			break;

		case 0:
			return;

		default:
			ret = display_pantalla(Iptr, 2);
		}
	}while (ret!=0);
}

int preguntar (CString cstPregunta, int pos)
{
int saltar, ret;
CString cstLectura;
char tecla;

	clreol(0,pos);clreol(0,pos+1);
	gotoxy(0,pos);printf("%s", cstPregunta);

	saltar=0;
	do{
		gotoxy(0,pos+1);tecla=getkey();
		if (tecla==-12)
			saltar=1;

		if ((tecla=='s' || tecla=='S' ||
			 tecla=='n' || tecla=='N' || tecla=='c' || tecla=='C'))
			saltar=1;
	}while (saltar==0);

	if (tecla=='s' || tecla=='S')
		ret = _TECLA_SI;
	if (tecla=='n' || tecla=='N')
		ret = _TECLA_NO;
	if (tecla=='c' || tecla=='C')
		ret = _TECLA_CANCEL;
	if (tecla==-12)
		ret = _TECLA_ESC;

	return ret;
}

int preguntar2 (CString cstPregunta, int pos)
{
int saltar, ret;
CString cstLectura;
char tecla;

	clreol(0,pos);clreol(0,pos+1);
	gotoxy(0,pos);printf("%s", cstPregunta);

	saltar=0;
	do{
		gotoxy(0,pos+1);tecla=getkey();
		if (tecla==-12)
			saltar=1;

		if(tecla==-1 || tecla==-2 || tecla==-3 || tecla==-4 || tecla==-6 || tecla==-73 || tecla==-81)
			saltar=1;
	}while (saltar==0);

	if (tecla==-1)
		ret = _TECLA_F1;
	if (tecla==-2)
		ret = _TECLA_F2;
	if (tecla==-3)
		ret = _TECLA_F3;
	if (tecla==-4)
		ret = _TECLA_F4;
	if (tecla==-6)
		ret = _TECLA_F6;

//	if (tecla==VK_PGUP)
//		_PAGINA_ACTUAL -= 1;

//	if (tecla==VK_PGDN)
//		_PAGINA_ACTUAL += 1;

	if (tecla=='c' || tecla=='C')
		ret = _TECLA_CANCEL;
	if (tecla==-12)
		ret = _TECLA_ESC;

	return ret;
}

int preguntar_cantidad (CString cstPregunta, int pos)
{
int saltar, ret;
CString cstLectura;
char tecla;

	clreol(0,pos);clreol(0,pos+1);
	gotoxy(0,pos);printf("%s", cstPregunta);

	saltar=0;
	do{
		gotoxy(0,pos+1);tecla=getkey();
		if (tecla==-9 || tecla==-4 || tecla=='3' || tecla==-12)
			saltar=1;

		if (tecla=='s' || tecla=='S' || tecla=='n' ||
			tecla=='N' || tecla=='c' || tecla=='C')
			saltar=1;
	}while (saltar==0);

	if (tecla=='s' || tecla=='S' || tecla==-9)
		ret = _TECLA_SI;
	if (tecla=='n' || tecla=='N' || tecla==-4)
		ret = _TECLA_NO;
	if (tecla=='c' || tecla=='C' || tecla=='3')
		ret = _TECLA_CANCEL;
	if (tecla==-12)
		ret = _TECLA_ESC;

	return ret;
}

int preguntar_cantidad2 (CString cstPregunta, int pos)
{
int saltar, ret;
CString cstLectura;
char tecla;

	clreol(0,pos);clreol(0,pos+1);
	gotoxy(0,pos);printf("%s", cstPregunta);

	saltar=0;
	do{
		gotoxy(0,pos+1);tecla=getkey();
		if (tecla==-9 || tecla==-4 || tecla==-6 || tecla=='3' || tecla==-12)
			saltar=1;

		if (tecla=='s' || tecla=='S' || tecla=='n' ||
			tecla=='N' || tecla=='c' || tecla=='C' ||
			tecla=='P' || tecla=='p')
			saltar=1;
	}while (saltar==0);

	if (tecla=='s' || tecla=='S' || tecla==-9)
		ret = _TECLA_SI;
	if (tecla=='n' || tecla=='N' || tecla==-4)
		ret = _TECLA_NO;
	if (tecla=='c' || tecla=='C' || tecla=='3')
		ret = _TECLA_CANCEL;
	if (tecla=='p' || tecla=='P' || tecla==-6)
		ret=-7;
	if (tecla==-12)
		ret = _TECLA_ESC;

	return ret;
}

void soloenter()
{
char tec;

	do{
		tec=getkey();
	}while (tec!=13);
}

int enteresc()
{
char tec;
int ret;

	do{
		tec=getkey();
	}while (tec!=13 && tec!=-12);

	if (tec==-12)
		ret=-12;
	if (tec==13)
		ret=0;

	return ret;
}

int _tmain(int argc, TCHAR* argv[], TCHAR* envp[])
{
	int nRetCode = 0;
	int ret, opc, siguiente_operacion;

	_variant_t vRetVal;
	_bstr_t szRetVal;
    char * cRetVal;
	
	CString lectura_operario, lectura_terminal;

	_variant_t * vUsuario = new _variant_t;
	_variant_t * vTerminal = new _variant_t;

	VARIANT VariableVariant;
	_variant_t * vInputs = new _variant_t;

	// initialize MFC and print and error on failure
	if (!AfxWinInit(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), 0))
	{
		// TODO: change error code to suit your needs
		cerr << _T("Fatal Error: MFC initialization failed") << endl;
		nRetCode = 1;
		return nRetCode;
	}

	initconsole();
	::CoInitialize(NULL);

	LPCTSTR szProradComId = _T("compradio.prorad"); // Funcion Calls from Here

	// Se controla el caso de las CLASSES COM dispara una excepción
	FlushConsoleInputBuffer(hIn);
	try
	{
		// Crear instancia clase COM cuyo ProgId es 'Componente Comp.proradio'
		clrscr();
		gotoxy(0,14);
		printf("Creando instancia %s", szProradComId);

		// El tipo por defecto empleado en los COMs son variable de tipo VARIANT.
		compradio::IproradPtr comInstance(szProradComId);

		opc = _INICIALIZAR;

		do
		{
			switch(opc)
			{
			case _INICIALIZAR:
				// Conectar la BBDD
				clrscr();
				gotoxy(0,14);
				printf("Conectando");
				vRetVal = comInstance->inicioconexion();
				vRetVal.ChangeType (VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// OK
				case 'S':
					opc =_LECTURA_OPERARIO;
					break;

				case 'C':
					UserError(comInstance);
					nRetCode = 2;
					opc = _FINALIZAR;
					break;

				// Resto de casos: OK
				default:
					opc =_LECTURA_OPERARIO;
					break;
				}
				break;

			// Entrar código de operario
			case _LECTURA_OPERARIO:
				clrscr();
				gotoxy(0, 1); printf("PRORAD.2");
				gotoxy(0, 4); printf("Operario: ");

				lectura_operario="";
				gotoxy(10, 4); ret = leer_char(&lectura_operario, 4);

				switch(ret)
				{
				// Salir del programa
				case _TECLA_ESC:
					opc = _FINALIZAR;
					break;

				// Pedir terminal
				case _TECLA_0:
					vUsuario->SetString((const char *)lectura_operario);
					opc = _VALIDAR_OPERARIO;
					break;

				// Resto de casos: ignorar
				default:
					opc =_LECTURA_OPERARIO;
					break;
				}
				break;

			// Entrar tipo terminal
			case _LECTURA_TERMINAL:
				clrscr();
				gotoxy(0, 1); printf("PRORAD.2");
				gotoxy(0, 4); printf("Operario: %s", lectura_operario);
				gotoxy(0, 5); printf("Terminal: ");

				lectura_terminal = "";
				gotoxy(10, 5); ret = leer_char(&lectura_terminal, 2);

				switch(ret)
				{
				// Ir a pedir operario
				case _TECLA_ESC:
					siguiente_operacion = _LECTURA_OPERARIO;
					opc = _DESCONECTAR_OPERARIO;
					break;

				// Realizar las validaciones
				case _TECLA_0:
					vTerminal->SetString((const char *)lectura_terminal);
					opc = _VALIDAR_TERMINAL;
					break;

				// Resto de casos: ignorar
				default:
					opc =_LECTURA_TERMINAL;
					break;
				}
				break;

			// Validar operario
			case _VALIDAR_OPERARIO:
				vRetVal=comInstance->conectarusuario(vUsuario);
				vRetVal.ChangeType(VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// OK: Mostar menú de opciones
				case 'S':
					opc = _LECTURA_TERMINAL;
					break;

				// Valores incorrectos
				case 'N':
					UserError(comInstance);
					opc = _LECTURA_OPERARIO;
					break;

				// Error
				case 'C':
					UserError(comInstance);
					nRetCode = 3;
					opc = _FINALIZAR;
					break;

				// Resto de casos: ignorar
				default:
					opc =_LECTURA_OPERARIO;
					break;
				}
				break;

			// Validar terminal
			case _VALIDAR_TERMINAL:
				vRetVal=comInstance->scrvalidarterminal(vTerminal);
				vRetVal.ChangeType(VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// OK: Mostar menú de opciones
				case 'S':
					opc =_DATOS_OK;
					break;

				// Valores incorrectos
				case 'N':
					UserError(comInstance);
					opc = _LECTURA_TERMINAL;
					break;

				// Error
				case 'C':
					UserError(comInstance);
					nRetCode = 4;
					opc = _FINALIZAR;
					break;

				// Resto de casos: ignorar
				default:
					opc =_LECTURA_TERMINAL;
					break;
				}
				break;

			// Desconectar operario activo
			case _DESCONECTAR_OPERARIO:
				vRetVal = comInstance->desconectarusuario();
				vRetVal.ChangeType(VT_BSTR);

				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// Siguiente operación a realizar:
				//	- Pedir operario
				//	- Cerrar BBDD
				case 'S':
					opc = siguiente_operacion;
					break;

				// Error
				case 'C':
					UserError(comInstance);
					nRetCode = 5;
					opc = _FINALIZAR;
					break;

				// Resto valores: Error
				default:
					UserError(comInstance);
					nRetCode = 6;
					opc = _FINALIZAR;
					break;
				}
				break;

			// Display del menú general
			case _DATOS_OK:
				// Cargar la línea inicial de inputs.
				comInstance->get_lineainputs(vInputs);
				vInputs->ChangeType(VT_INT);
				VariableVariant=vInputs->Detach();
				_LINEA_INPUTS = (int)VariableVariant.bstrVal;

				// Cargar la columna de inputs.
				comInstance->get_columnainputs(vInputs);
				vInputs->ChangeType(VT_INT);
				VariableVariant=vInputs->Detach();
				_COLUMNA_INPUTS = (int)VariableVariant.bstrVal;

				// Llamada al menú principal.
				menu(lectura_operario, comInstance);
				nRetCode = 0;
				opc = _LECTURA_TERMINAL;
				break;

			// Finalizar programa
			case _FINALIZAR:
				FlushConsoleInputBuffer(hIn);
				opc = _DESCONECTAR_BBDD;
				break;

			// Cerrar BBDD
			case _DESCONECTAR_BBDD:
				FlushConsoleInputBuffer(hIn);

				vRetVal = comInstance->finconexion();
				szRetVal = vRetVal;
				cRetVal = szRetVal;

				switch(cRetVal[0])
				{
				// Valores incorrectos
				case 'N':
					UserError(comInstance);
					break;

				// Error
				case 'C':
					UserError(comInstance);
					break;
				}

				opc = _NO_OPERACION;
				break;
			}
		}while (opc != _NO_OPERACION);

		// Liberar Instancias de la clase COM
		clreol(0,14);
		gotoxy(0,14);printf("comInstance");
		comInstance.Release();
		gotoxy(0,14);printf("coUninitialize");
		::CoUninitialize();
		clreol(0,14);gotoxy(0,14);printf("_exit(0)");
		_exit(0);
	}//EOF try

	catch (_com_error &e)
	{
		
		dump_com_error(e);
	}
		
	return nRetCode;
}

void dump_com_error( _com_error &e )
{
	_bstr_t bstrDescription(e.Description());
	_bstr_t bstrError;

	bstrError=e.ErrorMessage();
	LPCTSTR szError = bstrError;
	LPCTSTR szDescription = bstrDescription;

	// Log de Excepciones Ojo añadir TID i
	clrscr();
	gotoxy(0,11);printf("Ha ocurrido una excepcion:");
	gotoxy(0,12);printf("%s", szDescription);
	getkey();
	if (szDescription!="")
	{
		gotoxy(0,13);printf("%s",szError);
	}
	else
	{
		gotoxy(0,13);printf("'Imposible recoger el error'");
	}

	gotoxy(0,14);printf("El programa se cerrara");
	getkey();
	clreol(0,14);printf("CoUninitialize");
	::CoUninitialize();	
	clreol(0,14);printf("_cexit()");
	//_cexit();
	exit(0);
}

// (c) JLLF 2001.
void clrlin(unsigned int ncolumna, unsigned int nlinea)
{
DWORD dwCharsWritten;
COORD coord;

//	SetConsoleTextAttribute(hOut, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE );
	coord.X = (WORD) ncolumna;
    coord.Y = (WORD) nlinea;
	FillConsoleOutputCharacter(hOut,(TCHAR) ' ', nLengthLin, coord, &dwCharsWritten);
    gotoxy(0, 0);
}
