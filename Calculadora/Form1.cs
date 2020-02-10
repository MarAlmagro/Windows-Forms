using System;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using calc;
 
namespace CalculadoraLinkia
{
    
    public partial class fm_calculadora : Form
    {
       
        private const float MAX_VALOR = 99999;
        private Calculadora calc = new Calculadora();
        Boolean nuevoOperando = false;
        Boolean limpiarHistorial = false;
        private Button[] numeros;

        public fm_calculadora()
        {
            // Establecer configuración números al formato español, 
            // independientemente de lo que tenga configurado el usuario
            // o el sistema operativo
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
            InitializeComponent();
            numeros = new Button[] {bt_0, bt_1, bt_2, bt_3, bt_4, bt_5, bt_6, bt_7, bt_8, bt_9};
        }


        // Borrar último carácter de la pantalla
        private void bt_c_Click(object sender, EventArgs e)
        {
            // Quitar foco al botón pulsado para evitar
            // interferencias con eventos teclado
            lb_pantalla.Focus();

            String text = lb_pantalla.Text;

            if (nuevoOperando)
                return;

            // Si en la pantalla hay un numero de -9 a 9 
            // se muestra un cero
            if (text.Trim('-').Length.Equals(1))
            {
                lb_pantalla.Text = "0";                
            }
            else
            {
                // En otro caso, borrar el último carácter del texto pantalla
                text = text.Substring(0, text.Length - 1);

                if (text.Equals("-0"))
                    text = "0";

                lb_pantalla.Text = string.Format("{0:#,0.##}", text);
            }            
                
        }

    
        // Cambiar signo al número que hay en la pantalla cacluladora
        private void cambiarSigno(object sender, EventArgs e)
            {
                // Quitar foco al botón pulsado para evitar
                // interferencias con eventos teclado
                lb_pantalla.Focus();

                String text = lb_pantalla.Text;

                if (text.StartsWith("-"))
                {
                    lb_pantalla.Text = text.Trim('-');
                    return;
                }
                if (! text.Equals("0"))
                    lb_pantalla.Text = "-" + text;
            }

       
        private void ponerComa(object sender, EventArgs e)
        {
            // Quitar foco al botón pulsado para evitar
            // interferencias con eventos teclado
            lb_pantalla.Focus();

            if (nuevoOperando)
                return;

            String text = lb_pantalla.Text.Trim('-');

            //Si ya hay una coma dejamos contenido pantalla como está
            if (text.Contains(",") || text.Equals(MAX_VALOR.ToString()) )
                return;

                lb_pantalla.Text += ",";
        }

        // Resetear calculadora
        private void borrarTodo(object sender, EventArgs e)
        {
            // Quitar foco al botón pulsado para evitar
            // interferencias con eventos teclado
            lb_pantalla.Focus();

            // Resetear historial de operaciones y pantalla
            // calculadora
            lb_pantalla.Text = "0";
            Tx_Historial.Text = "";

            // Vaciar calculadora
            calc.operar();

            // Resetear variables booleanas
            nuevoOperando = false;
            limpiarHistorial = false;
        }

        // Cuando se pulsa botón 0..9
        private void clickNumero(object sender, EventArgs e)
        {
            // Quitar foco al botón pulsado para evitar
            // interferencias con eventos teclado
            lb_pantalla.Focus();

            String text = lb_pantalla.Text;

            if (nuevoOperando)
            {
                nuevoOperando = false;
                text = "0";
            }

            if (limpiarHistorial)
            {
                Tx_Historial.Text = "";
                limpiarHistorial = false;
            }

            text += ((Button)sender).Text;

            // Si se supera el valor máximo permitido
            if (float.Parse(text.Trim('-')) > MAX_VALOR)            
                return;

            int posComa = text.IndexOf(',');
            // Si no tiene parte decimal
            if (posComa < 0)
                 lb_pantalla.Text = string.Format("{0:#,0.##}", float.Parse(text));
            else
            {
                // limitar parte decimal a un máximo de dos dígitos
                String parteDecimal = text.Substring(posComa + 1);
                if (parteDecimal.Length <= 2)
                     lb_pantalla.Text = string.Format("{0:#,0.##}", text);

            }

        }

        private void click_signo(object sender, EventArgs e)
        {
            // Quitar foco al botón pulsado para evitar
            // interferencias con eventos teclado
            lb_pantalla.Focus();

            // Obtener operando
            String text = lb_pantalla.Text; 
     
            // Si el operando en pantalla es NaN
            // le asignamos valor cero
            if (text.Trim('-').Equals("NaN"))
            {
                text = "0";
                lb_pantalla.Text = "0";
            }

            float num = float.Parse(text);

            // Evitar que los operandos superen valor máximo permitido
            if (num > MAX_VALOR)
            {
                text = MAX_VALOR.ToString();
                lb_pantalla.Text = string.Format("{0:#,0.##}", text);
            }

            // Evitar que los operandos superen valor mínimo permitido
            if (num < -MAX_VALOR)
            {
                text = "-" + MAX_VALOR.ToString();
                lb_pantalla.Text = string.Format("{0:#,0.##}", text);
            }


            // Evitar que el operando tenga más de dos decimales
            int posComa = text.IndexOf(',');
            if (posComa > 0)
            {
                String parteDecimal = text.Substring(posComa + 1);
                if (parteDecimal.Length > 2)
                {
                    text = text.Substring(0, posComa + 3);
                    lb_pantalla.Text = string.Format("{0:#,0.##}", text);
                }

            }

            // Insertar en calculadora el operando
            calc.insert_num(float.Parse(text));

            // Obtener operador pulsado
            string operador = ((Button)sender).Text;

            if (operador.Equals("="))
            {
                // Actualizar historial
                Tx_Historial.Text = calc.oper_actual() + operador;
                // Realizar operación
                float resultado = calc.operar();

                // Mostrar resultado
                lb_pantalla.Text = string.Format("{0:#,0.##}", resultado);

                // Actualizar historial de operaciones
                Tx_Historial.Text += lb_pantalla.Text;

                // Operación finalizada. La próxima acción
                // del usuario borrará el histórico operaciones
                limpiarHistorial = true;
            }
            else
            { 
                // Insertar en calculadora el operador pulsado
                calc.insert_op(char.Parse(operador));
                // Actualizar historial operaciones
                Tx_Historial.Text = calc.oper_actual();
                // Operación en curso, evitar que se borre el contenido
                // del histórico operaciones
                if (limpiarHistorial)
                    limpiarHistorial = false;
            }


            // Se espera que la próxima acción del usuario sea
            // introducir un nuevo operando
            nuevoOperando = true;

        }

        private void evento_teclado(object sender, KeyPressEventArgs e)
        {

            char c = e.KeyChar; 
            
            if (Char.IsDigit(c))
            {
                int index = int.Parse(e.KeyChar.ToString());
                numeros[index].PerformClick();
                return;
            }

            if (c == (char)Keys.Enter)
            {
                bt_igual.PerformClick();
                return;
            }

            
            switch (c)
            {
                case '+':
                    bt_mas.PerformClick();
                    break;
                case '-':
                    bt_menos.PerformClick();
                    break;
                case '/':
                    bt_div.PerformClick();
                    break;
                case '*':
                    bt_por.PerformClick();
                    break;
                case '=':
                    bt_igual.PerformClick();
                    break;
                case ',':
                    bt_coma.PerformClick();
                    break;
                case '.':
                    bt_coma.PerformClick();
                    break;
            }
        }

        private void evento_tecla_Supr(object sender, KeyEventArgs e)
        {
            
            if ( (e.KeyCode == Keys.Delete) || (e.KeyCode == Keys.Back) )
                bt_c.PerformClick();

        }

    }
}
