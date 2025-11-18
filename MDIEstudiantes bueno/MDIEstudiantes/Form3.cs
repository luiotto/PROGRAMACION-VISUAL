using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq; // NECESARIO para usar FirstOrDefault()
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIEstudiantes
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        // Método auxiliar para recargar la DataGridView
        private void ActualizarDataGridView()
        {
            dgvDatos.Rows.Clear();

            // Asegúrate de que la DataGridView (dgvDatos) tenga ahora 4 columnas definidas:
            // 0: Carnet, 1: Nombre, 2: Promedio, 3: Asignaturas
            foreach (var est in DatosCompartidos.ListaEstudiantes)
            {
                // 1. Obtener los nombres de las asignaturas
                // Se asume que 'est.Asignaturas' es una colección de objetos y cada objeto tiene una propiedad 'Nombre' o similar.
                // Si la estructura de datos es diferente, esta línea debe ajustarse.
                string asignaturasTexto = string.Join(", ", est.Asignaturas.Select(a => a.Nombre));

                // 2. Agregar la nueva columna de Asignaturas a la DataGridView
                dgvDatos.Rows.Add(est.Carnet, est.Nombre, est.Promedio().ToString("0.00"), asignaturasTexto);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            ActualizarDataGridView();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // (Dejado sin cambios)
        }

        // --- Manejador para el botón "Eliminar Un Estudiante" ---
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Verificar si hay alguna fila seleccionada.
            if (dgvDatos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un estudiante de la lista para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Obtener la fila seleccionada y el carnet (columna 0)
            DataGridViewRow filaSeleccionada = dgvDatos.SelectedRows[0];
            string carnetAEliminar = filaSeleccionada.Cells[0].Value.ToString();

            // 3. Confirmación del usuario
            DialogResult respuesta = MessageBox.Show(
                $"¿Está seguro de que desea eliminar al estudiante con carnet '{carnetAEliminar}'?",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                // 4. Buscar y eliminar al estudiante de la lista compartida
                var estudianteAEliminar = DatosCompartidos.ListaEstudiantes.FirstOrDefault(est => est.Carnet == carnetAEliminar);

                if (estudianteAEliminar != null)
                {
                    // Elimina de la lista de origen (DatosCompartidos)
                    DatosCompartidos.ListaEstudiantes.Remove(estudianteAEliminar);

                    // 5. Actualizar la DataGridView para reflejar el cambio
                    ActualizarDataGridView();
                    MessageBox.Show("Estudiante eliminado con éxito.", "Eliminación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("El estudiante no se encontró en la lista compartida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- Manejador para el botón "Borrar Todos" ---
        private void btnBorrarTodos_Click(object sender, EventArgs e)
        {
            if (DatosCompartidos.ListaEstudiantes == null || DatosCompartidos.ListaEstudiantes.Count == 0)
            {
                MessageBox.Show("La lista de estudiantes ya está vacía.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. Confirmación del usuario para borrado masivo
            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea borrar TODOS los estudiantes de la lista? Esta acción no se puede deshacer.",
                "Confirmar Borrado Masivo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta == DialogResult.Yes)
            {
                // 2. Limpiar la lista compartida
                DatosCompartidos.ListaEstudiantes.Clear();

                // 3. Actualizar la DataGridView
                ActualizarDataGridView();

                MessageBox.Show("Todos los estudiantes han sido borrados.", "Borrado Completo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}