using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Asegúrate de que esta directiva esté presente

namespace OrdenamientoMultihilo
{
    public partial class Form1 : Form
    {
        // ... (Variables globales - No cambian) ...
        private List<int> listaOriginal;
        private List<int> listaBurbuja;
        private List<int> listaQuick;
        private List<int> listaMerge;
        private List<int> listaSelection;

        private Thread hiloBurbuja;
        private volatile bool cancelarBurbuja = false;

        private Stopwatch relojBurbuja = new Stopwatch();
        private Stopwatch relojQuick = new Stopwatch();
        private Stopwatch relojMerge = new Stopwatch();
        private Stopwatch relojSelection = new Stopwatch();

        private List<string> logBurbuja = new List<string>();
        private List<string> logQuick = new List<string>();
        private List<string> logMerge = new List<string>();
        private List<string> logSelection = new List<string>();

        public Form1()
        {
            InitializeComponent();

            if (backgroundWorkerQuickSort != null)
            {
                backgroundWorkerQuickSort.WorkerReportsProgress = true;
                backgroundWorkerQuickSort.WorkerSupportsCancellation = true;
            }
            if (backgroundWorkerMergeSort != null)
            {
                backgroundWorkerMergeSort.WorkerReportsProgress = true;
                backgroundWorkerMergeSort.WorkerSupportsCancellation = true;
            }
            if (backgroundWorkerSelectionSort != null)
            {
                backgroundWorkerSelectionSort.WorkerReportsProgress = true;
                backgroundWorkerSelectionSort.WorkerSupportsCancellation = true;
            }

            txtCantidad.Text = "20000";
        }

        // ====================================================================
        // MANEJO DE LA INTERFAZ Y DATOS
        // ====================================================================

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            int cantidad = 0;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese un número válido y positivo de elementos a generar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Random rnd = new Random();
            listaOriginal = new List<int>(cantidad);
            for (int i = 0; i < cantidad; i++)
                listaOriginal.Add(rnd.Next(0, 1000000));

            MessageBox.Show($"Lista generada correctamente con {cantidad} números aleatorios.", "Generación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // CORREGIDO: Usamos try-catch simple para limpiar el gráfico y evitar el error
            try
            {
                chartTimes.Series["Tiempos"].Points.Clear();
            }
            catch { }

            progressBurbuja.Value = 0; lblBurbuja.Text = "Burbuja: 0%";
            progressQuickSort.Value = 0; lblQuickSort.Text = "QuickSort: 0%";
            progressMerge.Value = 0; lblMerge.Text = "Merge: 0%";
            progressSelection.Value = 0; lblSelection.Text = "Selection: 0%";

            btnIniciar.Enabled = true;
            btnGuardarWord.Enabled = false;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (listaOriginal == null || listaOriginal.Count == 0)
            {
                MessageBox.Show("Primero debe generar los datos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            logBurbuja.Clear(); logQuick.Clear(); logMerge.Clear(); logSelection.Clear();

            listaBurbuja = new List<int>(listaOriginal);
            listaQuick = new List<int>(listaOriginal);
            listaMerge = new List<int>(listaOriginal);
            listaSelection = new List<int>(listaOriginal);

            cancelarBurbuja = false;
            hiloBurbuja = new Thread(new ThreadStart(OrdenarBurbuja));
            hiloBurbuja.IsBackground = true;
            hiloBurbuja.Start();

            backgroundWorkerQuickSort.RunWorkerAsync(listaQuick);
            backgroundWorkerMergeSort.RunWorkerAsync(listaMerge);
            backgroundWorkerSelectionSort.RunWorkerAsync(listaSelection);

            btnIniciar.Enabled = false;
            btnGenerar.Enabled = false;
            btnDetener.Enabled = true;
            btnGuardarWord.Enabled = false;
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            btnDetener.Enabled = false;

            cancelarBurbuja = true;
            backgroundWorkerQuickSort.CancelAsync();
            backgroundWorkerMergeSort.CancelAsync();
            backgroundWorkerSelectionSort.CancelAsync();

            Thread.Sleep(100);
            VerificarFinalizado();
        }

        private void VerificarFinalizado()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(VerificarFinalizado));
                return;
            }

            bool quickBusy = backgroundWorkerQuickSort.IsBusy;
            bool mergeBusy = backgroundWorkerMergeSort.IsBusy;
            bool selBusy = backgroundWorkerSelectionSort.IsBusy;
            bool burbujaAlive = hiloBurbuja != null && hiloBurbuja.IsAlive;

            if (!quickBusy && !mergeBusy && !selBusy && !burbujaAlive)
            {
                btnIniciar.Enabled = true;
                btnGenerar.Enabled = true;
                btnDetener.Enabled = false;
                btnGuardarWord.Enabled = true;

                ActualizarChartTiempos();
            }
        }

        private void ActualizarChartTiempos()
        {
            // Usamos LINQ (requiere 'using System.Linq;')
            if (!chartTimes.Series.Any(s => s.Name == "Tiempos"))
            {
                // Si no existe, no podemos continuar.
                return;
            }

            try
            {
                Series tiemposSeries = chartTimes.Series["Tiempos"];
                tiemposSeries.Points.Clear();

                long tb = relojBurbuja.ElapsedMilliseconds;
                long tq = relojQuick.ElapsedMilliseconds;
                long tm = relojMerge.ElapsedMilliseconds;
                long ts = relojSelection.ElapsedMilliseconds;

                if (tb > 0) tiemposSeries.Points.AddXY("Burbuja", tb);
                if (tq > 0) tiemposSeries.Points.AddXY("QuickSort", tq);
                if (tm > 0) tiemposSeries.Points.AddXY("MergeSort", tm);
                if (ts > 0) tiemposSeries.Points.AddXY("Selection", ts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la gráfica: " + ex.Message, "Error de Gráfico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarWord_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Rich Text Format (*.rtf)|*.rtf|Text File (*.txt)|*.txt";
            sfd.FileName = "ResultadosOrdenamiento.rtf";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                sw.WriteLine(@"{\rtf1\ansi\deff0");
                sw.WriteLine(@"{\fonttbl{\f0 Arial;}}");
                sw.WriteLine(@"\pard\sa200\sl276\slmult1\f0\fs24");
                sw.WriteLine(@"\b Resultados de Ordenamiento Multihilo\b0\par");
                sw.WriteLine($"Cantidad de elementos: {listaOriginal.Count}\\par");
                sw.WriteLine(@"\par");

                sw.WriteLine(@"\b Tiempos de Ejecucion\b0\par");
                sw.WriteLine($"Burbuja - {lblBurbuja.Text}\\par");
                sw.WriteLine($"QuickSort - {lblQuickSort.Text}\\par");
                sw.WriteLine($"Merge - {lblMerge.Text}\\par");
                sw.WriteLine($"Selection - {lblSelection.Text}\\par");
                sw.WriteLine(@"\par");

                EscribirLogsRTF(sw, "Burbuja", logBurbuja);
                EscribirLogsRTF(sw, "QuickSort", logQuick);
                EscribirLogsRTF(sw, "MergeSort", logMerge);
                EscribirLogsRTF(sw, "SelectionSort", logSelection);

                sw.WriteLine("}");
            }

            MessageBox.Show($"Resultados guardados en: {sfd.FileName}", "Guardado Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EscribirLogsRTF(StreamWriter sw, string nombreAlgoritmo, List<string> logs)
        {
            sw.WriteLine($@"\b Logs {nombreAlgoritmo} ({logs.Count} reportes)\b0\par");
            foreach (var l in logs)
            {
                sw.WriteLine(l.Replace(@"\", @"\\") + @"\par");
            }
            sw.WriteLine(@"\par");
        }


        // ====================================================================
        // IMPLEMENTACIÓN DE BUBBLE SORT (THREAD)
        // ====================================================================

        private void OrdenarBurbuja()
        {
            relojBurbuja.Restart();
            int n = listaBurbuja.Count;
            bool completed = true;

            for (int i = 0; i < n - 1; i++)
            {
                if (cancelarBurbuja)
                {
                    completed = false;
                    break;
                }

                for (int j = 0; j < n - i - 1; j++)
                {
                    if (listaBurbuja[j] > listaBurbuja[j + 1])
                    {
                        int temp = listaBurbuja[j];
                        listaBurbuja[j] = listaBurbuja[j + 1];
                        listaBurbuja[j + 1] = temp;
                    }
                }

                if (i % 1000 == 0 || i == n - 2)
                {
                    int progreso = (int)(((i + 1) / (float)n) * 100);
                    if (!cancelarBurbuja)
                        logBurbuja.Add($"Iter {i}: {progreso}% - {relojBurbuja.ElapsedMilliseconds} ms");

                    this.Invoke(new Action(() =>
                    {
                        progressBurbuja.Value = Math.Min(progreso, 100);
                        lblBurbuja.Text = $"Burbuja: {progreso}%";
                    }));
                }
            }

            relojBurbuja.Stop();

            this.Invoke(new Action(() =>
            {
                string status = completed ? "Completado" : "Cancelado";
                progressBurbuja.Value = completed ? 100 : progressBurbuja.Value;
                lblBurbuja.Text = $"Burbuja: {status} en {relojBurbuja.ElapsedMilliseconds} ms";
                VerificarFinalizado();
            }));
        }


        // ====================================================================
        // Implementaciones de QuickSort, MergeSort y SelectionSort
        // ====================================================================

        private void QuickSort(List<int> lista, int izquierda, int derecha, BackgroundWorker worker, List<string> log)
        {
            if (worker.CancellationPending) return;
            if (izquierda < derecha)
            {
                int pivot = Particionar(lista, izquierda, derecha);
                QuickSort(lista, izquierda, pivot - 1, worker, log);
                QuickSort(lista, pivot + 1, derecha, worker, log);
            }

            if (lista.Count > 10000 && derecha % (lista.Count / 10) == 0)
            {
                int progreso = (int)(((float)derecha / lista.Count) * 100);
                log.Add($"Pos {derecha}: {progreso}% - {relojQuick.ElapsedMilliseconds} ms");
                worker.ReportProgress(Math.Min(progreso, 100));
            }
        }

        private int Particionar(List<int> lista, int izquierda, int derecha)
        {
            int pivote = lista[derecha];
            int i = izquierda - 1;
            for (int j = izquierda; j < derecha; j++)
            {
                if (lista[j] <= pivote)
                {
                    i++;
                    int temp = lista[i];
                    lista[i] = lista[j];
                    lista[j] = temp;
                }
            }
            int temp2 = lista[i + 1];
            lista[i + 1] = lista[derecha];
            lista[derecha] = temp2;
            return i + 1;
        }

        private void backgroundWorkerQuickSort_DoWork(object sender, DoWorkEventArgs e)
        {
            relojQuick.Restart();
            List<int> lista = (List<int>)e.Argument;
            QuickSort(lista, 0, lista.Count - 1, backgroundWorkerQuickSort, logQuick);
            relojQuick.Stop();

            if (backgroundWorkerQuickSort.CancellationPending) e.Cancel = true;
            else e.Result = relojQuick.ElapsedMilliseconds;
        }

        private void backgroundWorkerQuickSort_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressQuickSort.Value = e.ProgressPercentage;
            lblQuickSort.Text = $"QuickSort: {e.ProgressPercentage}%";
        }

        private void backgroundWorkerQuickSort_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) lblQuickSort.Text = $"QuickSort: Cancelado en {relojQuick.ElapsedMilliseconds} ms";
            else if (e.Error != null) { lblQuickSort.Text = "QuickSort: Error"; MessageBox.Show("Error en QuickSort: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else { lblQuickSort.Text = $"QuickSort: Completado en {e.Result} ms"; progressQuickSort.Value = 100; }
            VerificarFinalizado();
        }

        private List<int> Merge(List<int> left, List<int> right)
        {
            List<int> result = new List<int>(left.Count + right.Count);
            int i = 0, j = 0;
            while (i < left.Count && j < right.Count)
            {
                if (left[i] <= right[j]) { result.Add(left[i]); i++; }
                else { result.Add(right[j]); j++; }
            }
            while (i < left.Count) { result.Add(left[i]); i++; }
            while (j < right.Count) { result.Add(right[j]); j++; }
            return result;
        }

        private List<int> MergeSortAlgorithm(List<int> lista, BackgroundWorker worker, List<string> log, int totalCount)
        {
            if (worker.CancellationPending) return lista;
            if (lista.Count <= 1) return lista;

            int mid = lista.Count / 2;
            var left = MergeSortAlgorithm(lista.GetRange(0, mid), worker, log, totalCount);
            var right = MergeSortAlgorithm(lista.GetRange(mid, lista.Count - mid), worker, log, totalCount);
            var merged = Merge(left, right);

            if (merged.Count > 0 && totalCount > 0)
            {
                int progreso = (int)((merged.Count / (float)totalCount) * 100);
                if (progreso > progressMerge.Value && merged.Count % 5000 == 0)
                {
                    log.Add($"Merged size {merged.Count}: {progreso}% - {relojMerge.ElapsedMilliseconds} ms");
                    worker.ReportProgress(Math.Min(progreso, 100));
                }
            }
            return merged;
        }

        private void backgroundWorkerMergeSort_DoWork(object sender, DoWorkEventArgs e)
        {
            relojMerge.Restart();
            List<int> lista = (List<int>)e.Argument;
            MergeSortAlgorithm(lista, backgroundWorkerMergeSort, logMerge, lista.Count);
            relojMerge.Stop();

            if (backgroundWorkerMergeSort.CancellationPending) e.Cancel = true;
            else e.Result = relojMerge.ElapsedMilliseconds;
        }

        private void backgroundWorkerMergeSort_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressMerge.Value = e.ProgressPercentage;
            lblMerge.Text = $"Merge: {e.ProgressPercentage}%";
        }

        private void backgroundWorkerMergeSort_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) lblMerge.Text = $"Merge: Cancelado en {relojMerge.ElapsedMilliseconds} ms";
            else if (e.Error != null) { lblMerge.Text = "Merge: Error"; MessageBox.Show("Error en MergeSort: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else { lblMerge.Text = $"Merge: Completado en {e.Result} ms"; progressMerge.Value = 100; }
            VerificarFinalizado();
        }

        private void SelectionSort(List<int> lista, BackgroundWorker worker, List<string> log)
        {
            int n = lista.Count;
            for (int i = 0; i < n - 1; i++)
            {
                if (worker.CancellationPending) return;
                int min = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (lista[j] < lista[min]) min = j;
                }
                int tmp = lista[min];
                lista[min] = lista[i];
                lista[i] = tmp;

                if (i % 1000 == 0)
                {
                    int progreso = (int)(((float)i / n) * 100);
                    log.Add($"Iter {i}: {progreso}% - {relojSelection.ElapsedMilliseconds} ms");
                    worker.ReportProgress(Math.Min(progreso, 100));
                }
            }
        }

        private void backgroundWorkerSelectionSort_DoWork(object sender, DoWorkEventArgs e)
        {
            relojSelection.Restart();
            List<int> lista = (List<int>)e.Argument;
            SelectionSort(lista, backgroundWorkerSelectionSort, logSelection);
            relojSelection.Stop();

            if (backgroundWorkerSelectionSort.CancellationPending) e.Cancel = true;
            else e.Result = relojSelection.ElapsedMilliseconds;
        }

        private void backgroundWorkerSelectionSort_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressSelection.Value = e.ProgressPercentage;
            lblSelection.Text = $"Selection: {e.ProgressPercentage}%";
        }

        private void backgroundWorkerSelectionSort_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) lblSelection.Text = $"Selection: Cancelado en {relojSelection.ElapsedMilliseconds} ms";
            else if (e.Error != null) { lblSelection.Text = "Selection: Error"; MessageBox.Show("Error en SelectionSort: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else { lblSelection.Text = $"Selection: Completado en {e.Result} ms"; progressSelection.Value = 100; }
            VerificarFinalizado();
        }

        private void chartTimes_Click(object sender, EventArgs e) { /* vacío */ }

        private void lblMerge_Click(object sender, EventArgs e)
        {

        }
    }
}