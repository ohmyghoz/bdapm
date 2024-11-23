using BDA.DataModel;
using DevExpress.PivotGrid;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace BDA.Helper
{


    public static class BDADXExt
    {

        public static DataGridBuilder<T> OSIDADataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, osida_master osida, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == osida.kode).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");
                caption = caption.Replace("Njop", "NJOP");
                caption = caption.Replace("Satu Rek", "1 Rekening");
                caption = caption.Replace("Percent", "Persentase");
                caption = caption.Replace("Bulanlaporan", "Persentase");
                if (row.ColumnName != "rowid" && row.ColumnName != "etl_date" && row.ColumnName != "dm_subtotal_nilai_agunan_dalam_satu_rek1" && row.ColumnName != "dm_jumlah_bulan_dari_akad_akhir1")
                {
                    var width = 150;
                    var format1 = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_nama_ljk")
                    {
                        width = 350;
                    }
                    if (osida.kode == "osida_kolektibilitas_hari_tunggakan")
                    {
                        if (row.ColumnName == "dm_subtotal_nilai_agunan_dalam_satu_rek")
                        {
                            row.ColumnName = "dm_subtotal_nilai_agunan_dalam_satu_rek1";
                            caption = "Subtotal Nilai Agunan Dalam Satu Rekening";
                        }
                    }
                    if (osida.kode == "osida_hapusbuku_satu_tahun")
                    {
                        if (row.ColumnName == "dm_jumlah_bulan_dari_akad_akhir")
                        {
                            row.ColumnName = "dm_jumlah_bulan_dari_akad_akhir1";
                            caption = "Jumlah Bulan Dari Akad Akhir";
                        }
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }
                    if (row.ColumnName == "dm_nama_id_debitur")
                    {
                        caption = "Nama Debitur";
                    }
                    if (row.ColumnName == "dm_nama_kantor_cabang")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_plafon_awal_fasilitas_terbesar")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_tunggakan")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            caption = "Total Tunggakan (Pokok + Bunga)";
                            width = 250;
                        }
                        else
                        {
                            caption = "Tunggakan (Pokok + Bunga)";
                            width = 200;
                        }

                    }
                    if (row.ColumnName == "dm_posisi_laporan_keuangan_terakhir")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_laba_rugi_bruto_0")
                    {
                        caption = "Laba / Rugi Bruto 0";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_laba_rugi_bruto_1")
                    {
                        caption = "Laba / Rugi Bruto 1";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_laba_rugi_bruto_2")
                    {
                        caption = "Laba / Rugi Bruto 2";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_laba_rugi_tahun_berjalan_0")
                    {
                        caption = "Laba / Rugi Tahun Berjalan 0";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_laba_rugi_tahun_berjalan_1")
                    {
                        caption = "Laba / Rugi Tahun Berjalan 1";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_laba_rugi_tahun_berjalan_2")
                    {
                        caption = "Laba / Rugi Tahun Berjalan 2";
                        width = 300;
                    }
                    if (row.ColumnName.Contains("dm_liabilitas_jangka_pendek"))
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_jumlah_hari_menunggak")
                    {
                        width = 200;
                    }
                    if (row.ColumnName == "dm_jml_agunan_aset")
                    {
                        caption = "Jumlah Agunan Aset Tetap";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_status_pengajuan")
                    {
                        caption = "Baru / Perpanjangan";
                    }
                    if (row.ColumnName == "dm_njop_nilai_wajar")
                    {
                        if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
                        {
                            caption = "NJOP / Nilai Wajar (Total Nilai Agunan)";
                            width = 300;
                        }
                        else
                        {
                            caption = "NJOP / Nilai Wajar";
                            width = 175;
                        }
                    }
                    if (row.ColumnName == "dm_njop_nilai_wajar_persentase")
                    {
                        caption = "NJOP / Nilai Wajar (%)";
                        width = 175;
                    }
                    if (row.ColumnName == "dm_menurut_penilai_independen")
                    {
                        caption = "Menurut Penilai Independen (Total Nilai Agunan)";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_menurut_pelapor_persentase")
                    {
                        if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
                        {
                            caption = "Menurut Pelapor (%) (Coverage Terhadap Baki Debet)";
                            width = 375;
                        }
                        else
                        {
                            caption = "Menurut Pelapor (%)";
                            width = 175;
                        }
                    }
                    if (row.ColumnName == "dm_jml_hari_tunggakan")
                    {
                        caption = "Jml Hari Tunggakan";
                        width = 175;
                    }
                    if (row.ColumnName == "dm_nama_pemilik_agunan")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_nilai_agunan_penilai_independen")
                    {
                        width = 275;
                    }
                    if (row.ColumnName == "dm_no_agunan")
                    {
                        caption = "Nomor Agunan";
                    }
                    if (row.ColumnName == "dm_tanggal_mulai")
                    {
                        caption = "Tgl Mulai";
                    }
                    if (row.ColumnName == "dm_tanggal_jatuh_tempo")
                    {
                        caption = "Tgl Jatuh Tempo";
                    }
                    if (row.ColumnName == "dm_tanggal_awal_kredit")
                    {
                        caption = "Tgl Awal Kredit";
                    }
                    if (row.ColumnName == "dm_kondisi")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_no_rekening")
                    {
                        caption = "Nomor Rekening";

                        if (osida.kode == "osida_tanpa_info_keu_mst")
                        {
                            caption = "Jumlah Rekening";
                        }
                    }
                    if (row.ColumnName == "dm_baru_perpanjangan")
                    {
                        caption = "Baru/Perpanjangan";
                    }
                    if (row.ColumnName == "dm_kolektibilitas_dpd")
                    {
                        caption = "Kolektibilitas (DPD)";
                    }
                    if (row.ColumnName == "dm_nonmissing_persenparipasu_dalam_satu_rek")
                    {
                        caption = "Non Missing Persentase Paripasu dalam 1 rekening";
                    }
                    if (row.ColumnName == "dm_no_akad_akhir")
                    {
                        caption = "Nomor Akad Akhir";
                    }
                    if (row.ColumnName == "dm_tanggal_mulai_m1")
                    {
                        caption = "Tanggal Mulai M-1";
                    }
                    if (row.ColumnName == "dm_tanggal_jatuh_tempo_m1")
                    {
                        caption = "Tanggal Jatuh Tempo M-1";
                    }
                    if (row.ColumnName == "dm_plafon_awal_m1")
                    {
                        caption = "Plafon Awal M-1";
                    }
                    if (row.ColumnName == "dm_baki_debet_m1")
                    {
                        caption = "Baki Debet (1 Bulan Lalu)";
                    }
                    if (row.ColumnName == "dm_baki_debet_m2")
                    {
                        caption = "Baki Debet (2 Bulan Lalu)";
                    }
                    if (row.ColumnName == "dm_bulanlaporan_plafon")
                    {
                        caption = "Bulan Laporan - Plafon";
                    }
                    if (row.ColumnName == "dm_bulanlaporan_bakidebet")
                    {
                        caption = "Bulan Laporan - Baki Debet";
                    }
                    if (row.ColumnName == "dm_bulanlalu_m1_plafon")
                    {
                        caption = "Bulan Lalu (M-1) - Plafon";
                    }
                    if (row.ColumnName == "dm_bulanlalu_m1_bakidebet")
                    {
                        caption = "Bulan Lalu (M-1) - Baki Debet";
                    }
                    if (row.ColumnName == "dm_bulansebelumnya_m2_plafon")
                    {
                        caption = "Bulan Sebelumnya (M-2) - Plafon";
                    }
                    if (row.ColumnName == "dm_bulansebelumnya_m2_bakidebet")
                    {
                        caption = "Bulan Sebelumnya (M-2) - Baki Debet";
                    }


                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format1 = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format1 = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    //if (row.DataType == "decimal")
                    //{
                    //    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)
                    //    .DataType(colDataType).Format(format).CalculateFilterExpression("CFE"));
                    //}
                    //else
                    //{
                    //    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).DataType(colDataType).Format(format));
                    //}
                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true })
                    .DataType(colDataType).Format(format1));
                }
            }
            return grid;
        }

        public static DataGridBuilder<T> MADataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding")
                {
                    if (reportId == "ma_outstanding_macet_no_agunan")
                    {
                        if (row.ColumnName == "dm_lokasi_agunan" || row.ColumnName == "dm_jenis_agunan")
                        {
                            continue;
                        }
                    }
                    var width = 150;
                    var format = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }

                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;

                    }

                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).DataType(colDataType).Format(format));

                }
            }
            return grid;
        }
        public static DataGridBuilder<T> MacroDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();
            int vi = 1;
            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Fasilitas Pinjaman", "Kode Fasilitas Pinjaman");
                caption = caption.Replace("Cif", "CIF");
                caption = caption.Replace("Dpd", "DPD");
                if (row.ColumnName != "rowid")
                {
                    var width = 150;
                    var format = "";
                    var colDataType = GridColumnDataType.String;
                    if (reportId == "macro_output_forecast_level_ljk")
                    {
                        if (row.ColumnName == "dm_periode")
                        {
                            caption = "Periode Forecast";
                        }
                        if (row.ColumnName == "dm_pperiode")
                        {
                            caption = "Periode";
                        }
                    }
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }

                    if (row.ColumnName == "dm_outstanding_sekarang")
                    {
                        caption = "Nilai Outstanding Sekarang";
                    }
                    if (row.ColumnName == "dm_outstanding_sebelumnya")
                    {
                        if (reportId == "macro_pertumbuhan_pinjaman_level_ljk")
                        {
                            caption = "Nilai Outstanding Sebelumnya";
                        }
                        else
                        {
                            caption = "Nilai Outstanding Bulan Sebelumnya";
                        }

                    }
                    if (row.ColumnName == "dm_persen_pertumbuhan_outstanding")
                    {
                        caption = "Persentase Outstanding";
                    }
                    if (row.ColumnName == "dm_current_period")
                    {
                        caption = "Periode Sekarang";
                    }
                    if (row.ColumnName == "dm_prev_period")
                    {
                        caption = "Periode Sebelumnya";
                    }
                    if (row.ColumnName == "dm_jumlah_rekening_nonzero_os")
                    {
                        caption = "Total/Banyak Rekening yang Memiliki Outstanding Tidak 0";
                    }
                    if (row.ColumnName == "dm_prev_period")
                    {
                        caption = "Periode Sebelumnya";
                    }
                    if (row.ColumnName == "dm_avg_plafon_awal")
                    {
                        caption = "Rata-Rata Plafon Awal";
                    }
                    if (row.ColumnName == "dm_avg_plafon_efektif")
                    {
                        caption = "Rata-Rata Plafon Efektif";
                    }
                    if (row.ColumnName == "dm_avg_outstanding")
                    {
                        caption = "Rata-Rata Outstanding";
                    }
                    if (row.ColumnName == "dm_outstanding")
                    {
                        caption = "Nilai Outstanding";
                    }
                    if (row.ColumnName == "dm_outstanding_sebelumnya")
                    {
                        caption = "Nilai Outstanding Sebelumnya";
                    }
                    if (row.ColumnName == "dm_plafon_sebelumnya")
                    {
                        caption = "Nilai Plafon Sebelumnya";
                    }
                    if (row.ColumnName == "dm_kolektibilitas_sekarang")
                    {
                        caption = "Kolektibilitas Bulan Sekarang";
                    }
                    if (row.ColumnName == "dm_kode_jenis_kredit")
                    {
                        caption = "Jenis Kredit";
                    }
                    if (row.ColumnName == "dm_plafon")
                    {
                        caption = "Nilai Plafon";
                    }
                    if (row.ColumnName == "dm_jenis_pertumbuhan")
                    {
                        caption = "Jenis Persentase Outstanding";
                    }
                    if (row.ColumnName == "dm_persen_pertumbuhan_outstanding")
                    {
                        format = ".##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;

                        if (row.ColumnName == "dm_persen_pertumbuhan_outstanding")
                        {
                            format = "#,##0.###";
                            colDataType = GridColumnDataType.Number;
                        }

                    }

                    if (reportId == "macro_output_forecast_level_ljk")
                    {
                        if (row.ColumnName == "dm_pperiode")
                        {
                            vi = 0;
                        }
                    }
                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).VisibleIndex(vi)
                    .DataType(colDataType).Format(format));
                }
            }
            return grid;
        }

        public static DataGridBuilder<T> MicroDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");
                caption = caption.Replace("No Rekening", "Nomor Rekening");
                caption = caption.Replace("No Akad Awal", "Nomor Akad Awal");
                caption = caption.Replace("No Akad Akhir", "Nomor Akad Akhir");
                caption = caption.Replace("Outstanding", "Nilai Outstanding");
                caption = caption.Replace("Plafon", "Nilai Plafon");
                caption = caption.Replace("Baki Debet", "Nilai Baki Debet");
                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding")
                {
                    var width = 150;
                    var format = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }
                    if (row.ColumnName == "dm_kelas_plafon")
                    {
                        caption = "Kategori Kelas Plafon";
                    }
                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }


                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)
                    .DataType(colDataType).Format(format));
                }
            }
            return grid;
        }

        public static DataGridBuilder<T> MSDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding")
                {
                    var width = 150;
                    var format = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_list_ljk")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }

                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }


                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)
                    .DataType(colDataType).Format(format));
                }
            }
            return grid;
        }

        public static DataGridBuilder<T> DADataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding" && row.ColumnName != "dm_member_code")
                {
                    var width = 150;
                    var format1 = "";
                    var colDataType = GridColumnDataType.String;
                    string ht = "";

                    if (row.ColumnName == "dm_tanggal_cetak")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_tanggal_terima")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_segmentasi")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_list_ljk")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_baki_debet_bulan_data")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_cif")
                    {
                        colDataType = GridColumnDataType.String;
                    }
                    if (row.ColumnName == "dm_kode_jenis_id_debitur")
                    {
                        caption = "Kode Jenis Identitas Debitur";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_status_valid_id_debitur")
                    {
                        caption = "Status Valid Identitas Debitur";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_no_id_debitur")
                    {
                        caption = "Nomor Identitas Debitur";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_nama_id_debitur")
                    {
                        caption = "Nama Sesuai Identitas Debitur";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_lokasi_agunan")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_no_agunan")
                    {
                        caption = "Nomor Agunan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_kode_kantor_cabang")
                    {
                        caption = "Kantor Cabang";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_no_rekening")
                    {
                        caption = "Nomor Rekening";
                    }
                    if (row.ColumnName == "dm_persen_paripasu")
                    {
                        caption = "Persentase Paripasu";
                    }
                    if (row.ColumnName == "dm_status_valid_gender_based_kode")
                    {
                        caption = "Status Valid Gender Berdasarkan Kode";
                        width = 260;
                    }
                    if (row.ColumnName == "dm_status_valid_gender_based_ktp")
                    {
                        caption = "Status Valid Gender Berdasarkan KTP";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_similarity_threshold")
                    {
                        caption = "Similarity Threshold Score";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_final_similarity_method")
                    {
                        caption = "Similarity Function Method";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_kode_jenis_identitas_debitur")
                    {
                        caption = "Kode Jenis Identitas Debitur";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_identity_number_name_1")
                    {
                        caption = "Nama Sesuai Identitas Debitur - Perbandingan";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_nama_ibu_kandung_1")
                    {
                        caption = "Nama Ibu Kandung - Perbandingan";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_dokumen_kepemilikan_agunan")
                    {
                        caption = "Dokumen Kepemilikan Agunan";
                        width = 250;
                    }

                    if (row.ColumnName == "dm_jenis_ljk_1")
                    {
                        caption = "Jenis LJK - Perbandingan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_kode_ljk_1")
                    {
                        caption = "Kode LJK - Perbandingan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_cif_1")
                    {
                        caption = "CIF - Perbandingan";
                        width = 250;
                    }

                    if (row.ColumnName == "dm_similarity_result" && reportId == "agunan_id_anomali")
                    {
                        caption = "Dugaan Duplikasi Penggunaan Agunan";
                        width = 275;
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {

                        if (reportId == "debitur_anomali_duplikasi_nama_debitur")
                        {
                            if (row.ColumnName == "dm_tanggal_lahir")
                            {
                                colDataType = GridColumnDataType.String;
                                ht = "<div id='tooltiptarget1'>" + caption + "</div>";
                            }
                            else
                            if (row.ColumnName == "dm_tanggal_lahir_1")
                            {
                                colDataType = GridColumnDataType.String;
                                caption = "Tanggal Lahir - Perbandingan";
                                width = 250;
                                ht = "<div id='tooltiptarget2'>" + caption + "</div>";
                            }
                        }
                        else
                        if (reportId == "debitur_anomali_gender")
                        {
                            if (row.ColumnName == "dm_tanggal_lahir")
                            {
                                colDataType = GridColumnDataType.Date;
                                ht = "<div id='tooltiptarget1'>" + caption + "</div>";

                                if (isHive == true)
                                {
                                    colDataType = GridColumnDataType.String;
                                }
                            }
                        }
                        else
                        {
                            width = 300;
                            colDataType = GridColumnDataType.Date;
                        }

                    }
                    //if (row.ColumnName == "dm_periode")
                    //{
                    //    colDataType = GridColumnDataType.Date;
                    //}

                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format1 = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format1 = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    if (ht != "")
                    {
                        grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true })
                        .DataType(colDataType).Format(format1).HeaderCellTemplate(ht));
                    }
                    else
                    {
                        grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true })
                        .DataType(colDataType).Format(format1));
                    }

                }
            }
            return grid;
        }

        public static DataGridBuilder<T> DA2023DataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");


                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding" && row.ColumnName != "dm_member_code")
                {
                    var width = 150;
                    var format1 = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_nama_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_jenis_agunan")
                    {
                        width = 200;
                    }
                    if (row.ColumnName == "dm_nama_lengkap")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_nama_badan_usaha")
                    {
                        width = 500;
                    }
                    if (row.ColumnName == "dm_nama_gadis_ibu_kandung")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_mother_maiden_name")
                    {
                        caption = "Nama Gadis Ibu Kandung";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_spouse_name")
                    {
                        caption = "Nama Pasangan";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_alamat")
                    {
                        width = 650;
                    }
                    if (row.ColumnName == "dm_nama_kantor_cabang")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_list_ljk")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_jenis_badan_usaha")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_tempat_pendirian")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_kode_jenis_badan_usaha")
                    {
                        width = 175;
                    }
                    if (row.ColumnName == "dm_no_akta_pendirian")
                    {
                        caption = "Nomor Akta Pendirian";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_no_akta_perubahan_terakhir")
                    {
                        caption = "Nomor Akta Perubahan Terakhir";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_keterangan")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_baki_debet_nominal")
                    {
                        caption = "Baki Debet / Nominal";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_penilai_independen")
                    {
                        caption = "Nama Penilai Independen";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_cif")
                    {
                        colDataType = GridColumnDataType.String;
                    }
                    if (row.ColumnName == "dm_jenis_debitur")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_segmen")
                    {
                        width = 225;
                    }
                    if (row.ColumnName == "dm_tempat_bekerja")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_alamat_tempat_bekerja")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_npwp")
                    {
                        caption = "NPWP";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_telepon_selular")
                    {
                        caption = "Nomor Telepon Selular";
                        width = 175;
                    }
                    if (row.ColumnName == "dm_kode_jenis_id_debitur")
                    {
                        caption = "Kode Jenis Identitas Debitur";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_status_valid_id_debitur")
                    {
                        caption = "Status Valid Identitas Debitur";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_no_id_debitur")
                    {
                        if (reportId == "da_anomali_format_npwp")
                        {
                            caption = "Identitas Badan Usaha";
                            width = 200;
                        }
                        else
                        if (reportId == "da_anomali_format_telepon_debitur" || reportId == "da_anomali_alamat_email_debitur")
                        {
                            caption = "Nomor NIK";
                            width = 200;
                        }
                        else
                        if (reportId == "da_anomali_bukti_kepemilikan_agunan")
                        {
                            caption = "Nomor Identitas";
                            width = 200;
                        }
                        else
                        {
                            caption = "No Identitas";
                            width = 200;
                        }
                    }
                    if (row.ColumnName == "dm_no_identitas_badan_usaha")
                    {
                        caption = "No Identitas Badan Usaha";
                        width = 220;
                    }
                    if (row.ColumnName == "dm_nama_id_debitur")
                    {
                        if (reportId == "da_anomali_format_npwp")
                        {
                            caption = "Nama Badan Usaha";
                            width = 550;
                        }
                        else
                        {
                            caption = "Nama Sesuai Identitas";
                            width = 550;
                        }
                    }
                    if (row.ColumnName == "dm_nama_sesuai_identitas")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_lembaga_pemeringkat")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_penghasilan_kotor_per_tahun")
                    {
                        width = 225;
                    }
                    if (row.ColumnName == "dm_kode_sumber_penghasilan")
                    {
                        width = 200;
                    }
                    if (row.ColumnName == "dm_nilai_agunan_menurut_pelapor")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_nilai_agunan_menurut_penilai_independen")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_nilai_agunan_wajar")
                    {
                        caption = "Nilai Agunan Wajar (NJOP)";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_lokasi_agunan")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_no_agunan")
                    {
                        caption = "Nomor Agunan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_kode_jenis_kredit_pembiayaan")
                    {
                        caption = "Kode Jenis Kredit/Pembiayaan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_kode_jenis_penggunaan")
                    {
                        width = 200;
                    }
                    if (row.ColumnName == "dm_suku_bunga_atau_imbalan")
                    {
                        caption = "Suku Bunga atau Imbalan";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_jenis_suku_bunga_atau_imbalan")
                    {
                        caption = "Jenis Suku Bunga atau Imbalan";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_no_rekening")
                    {
                        if (reportId == "da_anomali_nilai_agunan_deb" || reportId == "da_anomali_nilai_njop_agunan" || reportId == "da_anomali_format_peringkat_agunan" || reportId == "da_anomali_tingkat_suku_bunga" || reportId == "da_anomali_bukti_kepemilikan_agunan")
                        {
                            caption = "No Rekening";
                        }
                        else
                        {
                            caption = "Nomor Rekening";
                            width = 175;
                        }
                    }
                    if (row.ColumnName == "dm_persen_paripasu")
                    {
                        caption = "Persentase Paripasu";
                    }
                    if (row.ColumnName == "dm_status_valid_gender_based_kode")
                    {
                        caption = "Status Valid Gender Berdasarkan Kode";
                        width = 260;
                    }
                    if (row.ColumnName == "dm_status_valid_gender_based_ktp")
                    {
                        caption = "Status Valid Gender Berdasarkan KTP";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_similarity_threshold")
                    {
                        caption = "Similarity Threshold Score";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_final_similarity_method")
                    {
                        caption = "Similarity Function Method";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_kode_jenis_identitas_debitur")
                    {
                        caption = "Kode Jenis Identitas Debitur";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_identity_number_name_1")
                    {
                        caption = "Nama Sesuai Identitas Debitur - Perbandingan";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_nama_ibu_kandung_1")
                    {
                        caption = "Nama Ibu Kandung - Perbandingan";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_dokumen_kepemilikan_agunan")
                    {
                        caption = "Dokumen Kepemilikan Agunan";
                        width = 250;
                    }

                    if (row.ColumnName == "dm_jenis_ljk_1")
                    {
                        caption = "Jenis LJK - Perbandingan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_kode_ljk_1")
                    {
                        caption = "Kode LJK - Perbandingan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_cif_1")
                    {
                        caption = "CIF - Perbandingan";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_baki_debet_m1")
                    {
                        caption = "Baki Debet (1 Bln Lalu)";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_selisih_baki_debet_m1")
                    {
                        caption = "Selisih Baki Debet (1 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_percent_selisih_baki_debet_m1")
                    {
                        caption = "% Selisih Baki Debet (1 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_baki_debet_m3")
                    {
                        caption = "Baki Debet (3 Bln Lalu)";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_selisih_baki_debet_m3")
                    {
                        caption = "Selisih Baki Debet (3 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_percent_selisih_baki_debet_m3")
                    {
                        caption = "% Selisih Baki Debet (3 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_baki_debet_m6")
                    {
                        caption = "Baki Debet (6 Bln Lalu)";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_selisih_baki_debet_m6")
                    {
                        caption = "Selisih Baki Debet (6 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_percent_selisih_baki_debet_m6")
                    {
                        caption = "% Selisih Baki Debet (6 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_baki_debet_m12")
                    {
                        caption = "Baki Debet (12 Bln Lalu)";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_selisih_baki_debet_m12")
                    {
                        caption = "Selisih Baki Debet (12 Bln Lalu)";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_percent_selisih_baki_debet_m12")
                    {
                        caption = "% Selisih Baki Debet (12 Bln Lalu)";
                        width = 250;
                    }

                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        if (row.ColumnName == "dm_tanggal_akte_pendirian")
                        {
                            width = 200;
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        if (row.ColumnName == "dm_tanggal_akta_pendirian")
                        {
                            width = 200;
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        if (row.ColumnName == "dm_tanggal_akta_perubahan_terakhir")
                        {
                            width = 225;
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        {
                            width = 130;
                            colDataType = GridColumnDataType.Date;
                        }

                    }
                    //if (row.ColumnName == "dm_periode")
                    //{
                    //    colDataType = GridColumnDataType.Date;
                    //}

                    if (row.DataType == "date")
                    {
                        if (row.ColumnName == "dm_bulan_data" || row.ColumnName == "dm_periode")
                        {
                            if (isHive == false)
                            {
                                format1 = "yyyy-MM-dd";
                                colDataType = GridColumnDataType.Date;
                            }
                            else
                            {
                                format1 = "";
                                colDataType = GridColumnDataType.String;
                            }
                        }
                        else if (row.ColumnName == "dm_tanggal_lahir")
                        {
                            format1 = "dd-MM-yyyy";
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        {
                            format1 = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format1 = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true })
                    .DataType(colDataType).Format(format1));

                }
            }
            return grid;
        }

        public static DataGridBuilder<T> LADataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_kolektibilitas" && row.ColumnName != "dm_pperiode" && row.ColumnName != "dm_pperiode_inquiry" && row.ColumnName != "dm_pstatus_pengecekan")
                {
                    var width = 150;
                    var format1 = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_list_ljk")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_pstatus_pengecekan")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_jenis_fasilitas_pinjaman")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_jenis_kredit")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_no_akad_akhir")
                    {
                        caption = "Nomor Akad Akhir";
                        width = 250;
                    }
                    if (row.ColumnName == "dm_no_id_debitur")
                    {
                        caption = "Nomor ID Debitur";
                    }
                    if (row.ColumnName == "dm_status_pengecekan")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_algoritma_pencarian")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_inquiry_id")
                    {
                        caption = "Inquiry ID";
                    }
                    if (row.ColumnName == "dm_periode_inquiry")
                    {
                        caption = "Tanggal Inquiry";
                    }
                    if (row.ColumnName == "dm_created_by")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_nomor_laporan_inquiry")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_nama_kantor_cabang")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_status_inquiry")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_cek_sebelum_pencairan")
                    {
                        width = 175;
                    }
                    if (row.ColumnName == "dm_cek_setelah_pencairan")
                    {
                        width = 175;
                    }
                    if (row.ColumnName == "dm_cek_1bulan_sebelum_cair")
                    {
                        caption = "Cek 1 Bulan Sebelum Pencairan";
                        width = 210;
                    }
                    if (row.ColumnName == "dm_cek_1bulan_setelah_cair")
                    {
                        caption = "Cek 1 Bulan Setelah Pencairan";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_selisih_awalvsinquiry")
                    {
                        if (reportId == "la_inquiry_check")
                        {
                            caption = "Selisih Tanggal Awal Pinjaman terhadap Tanggal Inquiry";
                            width = 410;
                        }
                        else if (reportId == "la_inquiry_pattern_recognition")
                        {
                            caption = "Selisih Tanggal Awal Pinjaman Mulai terhadap Tanggal Inquiry";
                            width = 410;
                        }

                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;
                        colDataType = GridColumnDataType.Date;

                        if (row.ColumnName == "dm_tanggal_awal_pinjaman")
                        {
                            width = 175;
                        }

                    }
                    if (row.ColumnName == "dm_nilai_outstanding" || row.ColumnName == "dm_nilai_plafon" || row.ColumnName == "dm_nilai_baki_debet" || row.ColumnName == "dm_selisih_awalvsinquiry")
                    {
                        format1 = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }
                    else if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format1 = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    //else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    //{
                    //        format = ",##0";
                    //        colDataType = GridColumnDataType.Number;
                    //}

                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true })
                    .DataType(colDataType).Format(format1));
                }
            }
            return grid;
        }
        public static DataGridBuilder<T> CMDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding")
                {
                    var width = 150;
                    var format1 = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_ljk")
                    {
                        width = 350;
                    }
                    //if (row.ColumnName == "dm_list_ljk")
                    //{
                    //    width = 250;
                    //}
                    if (row.ColumnName == "dm_nama_debitur") width = 300;

                    if (row.ColumnName == "dm_dati1")
                    {
                        caption = "Provinsi";
                        width = 200;
                    }

                    if (row.ColumnName == "dm_dati2")
                    {
                        caption = "Kota / Kabupaten";
                        width = 250;
                    }

                    if (row.ColumnName == "dm_nilai")
                    {
                        colDataType = GridColumnDataType.Number;
                    }

                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;
                        colDataType = GridColumnDataType.Date;
                    }

                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format1 = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format1 = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true })
                    .DataType(colDataType).Format(format1));
                }
            }
            return grid;
        }
        public static DataGridBuilder<T> OsintDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding" && row.ColumnName != "dm_pday_scrapping")
                {
                    var width = 150;
                    var format = "";
                    int vi = 0;
                    var colDataType = GridColumnDataType.String;

                    if (row.ColumnName == "dm_pperiode_scrapping")
                    {
                        caption = "Periode Data";
                        //vi = 7;
                    }

                    if (row.ColumnName == "dm_text")
                    {
                        vi = 5;
                    }
                    if (row.ColumnName == "dm_clean_text")
                    {
                        width = 1300;
                        vi = 6;
                    }
                    if (row.ColumnName == "dm_snippet")
                    {
                        width = 1300;
                        vi = 4;
                    }
                    if (row.ColumnName == "dm_judul_berita")
                    {
                        caption = "Title";
                        width = 600;
                        vi = 2;
                    }
                    if (row.ColumnName == "dm_url")
                    {
                        caption = "Link";
                        width = 900;
                        vi = 3;
                    }
                    if (row.ColumnName == "dm_keyword")
                    {
                        caption = "Keyword";
                        width = 400;
                        vi = 1;
                    }
                    if (row.ColumnName == "dm_scrapping_time")
                    {
                        caption = "Scraped at";

                    }
                    if (row.ColumnName == "dm_pcategory_scrapping")
                    {
                        caption = "Category";
                        vi = 9;
                    }
                    if (row.ColumnName == "dm_pday_scrapping")
                    {
                        caption = "Pday";
                        vi = 8;
                    }
                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    if (row.DataType == "datetime")
                    {
                        if (isHive == false)
                        {
                            format = "yyyy-MM-dd HH:mm:ss";
                            colDataType = GridColumnDataType.DateTime;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    if (row.ColumnName == "dm_keyword")
                    {
                        grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)//.CellTemplate("<text><a style='cursor: pointer; ' href='#' onclick=\"var ss='{\"data\":\"<%= data.lem2%> \"}.replace(/" / g, '&quot;')' \" title=' <%= data.dm_keyword%>'><%= data.dm_keyword%></a><text>")
                        .DataType(colDataType).Format(format).VisibleIndex(vi));
                    }
                    else
                    {
                        grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)
                        .DataType(colDataType).Format(format).VisibleIndex(vi));
                    }

                }
            }
            return grid;
        }
        public static DataGridBuilder<T> DNADataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string reportId, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == reportId).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding")
                {
                    var width = 150;
                    var format1 = "";
                    int vi = 0;
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_number_of_facility_type")
                    {
                        vi = 5;
                        width = 200;
                    }
                    if (row.ColumnName == "dm_relationship")
                    {
                        vi = 1;
                    }
                    if (row.ColumnName == "dm_relationship_status")
                    {
                        vi = 4;
                    }
                    if (row.ColumnName == "dm_has_joint_credit")
                    {
                        vi = 6;
                    }
                    if (row.ColumnName == "dm_number_of_associated_rel")
                    {
                        vi = 7;
                        width = 200;
                    }
                    if (row.ColumnName == "dm_number_of_active_associated_rel")
                    {
                        vi = 8;
                        width = 250;
                    }

                    if (row.ColumnName == "dm_estimated_subtotal_active_collateral_value")
                    {
                        vi = 9;
                        width = 300;
                    }
                    if (row.ColumnName == "dm_estimated_subtotal_active_joint_collateral_value")
                    {
                        vi = 10;
                        width = 350;
                    }

                    if (row.ColumnName == "dm_active_joint_outstanding")
                    {
                        vi = 12;
                        width = 200;
                    }
                    if (row.ColumnName == "dm_periode")
                    {
                        caption = "Report Year Month";
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;
                        colDataType = GridColumnDataType.Date;

                    }

                    if (row.ColumnName == "dm_first_node_id")
                    {
                        vi = 2;
                        width = 650;
                    }
                    if (row.ColumnName == "dm_second_node_id")
                    {
                        vi = 3;
                    }
                    if (row.ColumnName == "dm_edge_width_factor")
                    {
                        vi = 13;
                    }
                    if (row.ColumnName == "dm_edge_color_factor")
                    {
                        vi = 14;
                    }

                    if (row.ColumnName == "dm_active_outstanding")
                    {
                        vi = 11;
                        colDataType = GridColumnDataType.Number;
                    }

                    if (row.DataType == "date")
                    {
                        if (isHive == false)
                        {
                            format1 = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                    }
                    if (row.DataType == "datetime")
                    {
                        if (isHive == false)
                        {
                            format1 = "yyyy-MM-dd HH:mm:ss";
                            colDataType = GridColumnDataType.DateTime;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format1 = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }


                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).EditorOptions(new { format = format1, showClearButton = true }).VisibleIndex(vi)
                    .DataType(colDataType).Format(format1));
                }
            }
            return grid;
        }

        public static DataGridBuilder<T> OSIDA2023DataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, osida_master osida, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == osida.kode).ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");
                caption = caption.Replace("Njop", "NJOP");

                if (row.ColumnName != "rowid" && row.ColumnName != "etl_date" && row.ColumnName != "dm_subtotal_nilai_agunan_dalam_satu_rek1" && row.ColumnName != "dm_jumlah_bulan_dari_akad_akhir1")
                {
                    var width = 150;
                    var format = "";
                    var colDataType = GridColumnDataType.String;
                    if (row.ColumnName == "dm_jenis_ljk")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_nama_ljk")
                    {
                        width = 400;
                    }
                    if (row.ColumnName == "dm_nama_debitur") width = 300;
                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }
                    if (row.ColumnName == "dm_nama_id_debitur")
                    {
                        caption = "Nama Debitur";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_nama_kantor_cabang")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_det_bu" || osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus" || osida.kode == "osida_pemberian_kur_deb_noneligible_det")
                        {
                            continue;
                        }
                        width = 300;
                    }
                    if (row.ColumnName == "dm_kode_kantor_cabang")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_det_bu" || osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus" || osida.kode == "osida_pemberian_kur_deb_noneligible_det")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_plafon_awal_fasilitas_terbesar")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_jenis_debitur")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_tunggakan")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            continue;
                        }
                        else
                        {
                            if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                            {
                                caption = "Total Tunggakan (Pokok + Bunga)";
                                width = 250;
                            }
                            else
                            {
                                caption = "Tunggakan (Pokok + Bunga)";
                                width = 200;
                            }
                        }


                    }
                    if (row.ColumnName == "dm_posisi_laporan_keuangan_terakhir")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_laba_rugi_bruto_0")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Laba / Rugi Bruto 0";
                        }

                    }
                    if (row.ColumnName == "dm_laba_rugi_bruto_1")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Laba / Rugi Bruto 1";
                        }
                    }
                    if (row.ColumnName == "dm_laba_rugi_bruto_2")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Laba / Rugi Bruto 2";
                        }
                    }
                    if (row.ColumnName == "dm_laba_rugi_tahun_berjalan_0")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Laba / Rugi Tahun Berjalan 0";
                            width = 250;
                        }

                    }
                    if (row.ColumnName == "dm_laba_rugi_tahun_berjalan_1")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Laba / Rugi Tahun Berjalan 1";
                            width = 250;
                        }
                    }
                    if (row.ColumnName == "dm_laba_rugi_tahun_berjalan_2")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Laba / Rugi Tahun Berjalan 2";
                            width = 250;
                        }
                    }
                    if (row.ColumnName == "dm_liabilitas_jangka_pendek_0")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Liabilitas Jangka Pendek 0";
                            width = 250;
                        }
                    }
                    if (row.ColumnName == "dm_liabilitas_jangka_pendek_1")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Liabilitas Jangka Pendek 1";
                            width = 250;
                        }
                    }
                    if (row.ColumnName == "dm_liabilitas_jangka_pendek_2")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "Liabilitas Jangka Pendek 2";
                            width = 250;
                        }
                    }
                    if (row.ColumnName == "dm_aset_lancar_0")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_aset_lancar_1")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_aset_lancar_2")
                    {
                        if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_nilai_agunan_penilai_independen")
                    {
                        width = 275;
                    }
                    if (row.ColumnName == "dm_jumlah_hari_menunggak")
                    {
                        width = 200;
                    }
                    if (row.ColumnName == "dm_jml_agunan_aset")
                    {
                        caption = "Jumlah Agunan Aset Tetap";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_jml_agunan")
                    {
                        caption = "Jumlah Agunan";
                    }
                    if (row.ColumnName == "dm_status_pengajuan")
                    {
                        caption = "Baru / Perpanjangan";
                    }
                    if (row.ColumnName == "dm_njop_nilai_wajar")
                    {
                        if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
                        {
                            continue;
                            //caption = "NJOP / Nilai Wajar (Total Nilai Agunan)";
                            //width = 300;
                        }
                        else
                        {
                            caption = "NJOP / Nilai Wajar";
                            width = 175;
                        }
                    }
                    if (row.ColumnName == "dm_njop_nilai_wajar_persentase")
                    {
                        if (osida.kode == "osida_agunan_tdk_dinilai_independen_mst")
                        {
                            continue;
                        }
                        else
                        {
                            caption = "NJOP / Nilai Wajar (%)";
                            width = 175;
                        }
                    }
                    if (row.ColumnName == "dm_menurut_penilai_independen")
                    {
                        if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
                        {
                            continue;
                            //caption = "NJOP / Nilai Wajar (Total Nilai Agunan)";
                            //width = 300;
                        }
                        caption = "Menurut Penilai Independen (Total Nilai Agunan)";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_menurut_pelapor")
                    {
                        if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
                        {
                            continue;
                            //caption = "Menurut Pelapor (%) (Coverage Terhadap Baki Debet)";
                            //width = 375;
                        }
                    }
                    if (row.ColumnName == "dm_menurut_pelapor_persentase")
                    {
                        if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
                        {
                            continue;
                            //caption = "Menurut Pelapor (%) (Coverage Terhadap Baki Debet)";
                            //width = 375;
                        }
                        else
                        {
                            caption = "Menurut Pelapor (%)";
                            width = 175;
                        }
                    }
                    if (row.ColumnName == "dm_kualitas")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_total_plafon")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_jabatan")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_pangsa_pemilik")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_total_baki_debet_bermasalah")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_total_tunggakan_bunga")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_nama_pengurus_pemilik")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_jml_hari_tunggakan")
                    {
                        caption = "Jml Hari Tunggakan";
                        width = 175;
                    }
                    if (row.ColumnName == "dm_nama_pemilik_agunan")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_no_agunan")
                    {
                        caption = "Nomor Agunan";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_tanggal_mulai")
                    {
                        caption = "Tgl Mulai";
                    }
                    if (row.ColumnName == "dm_tanggal_jatuh_tempo")
                    {
                        caption = "Tgl Jatuh Tempo";
                    }
                    if (row.ColumnName == "dm_tanggal_awal_kredit")
                    {
                        caption = "Tgl Awal Kredit";
                    }
                    if (row.ColumnName == "dm_tanggal_kondisi")
                    {
                        caption = "Tgl Kondisi";
                    }
                    if (row.ColumnName == "dm_tgl_lahir")
                    {
                        caption = "Tanggal Lahir";
                    }
                    if (row.ColumnName == "dm_no_identitas")
                    {
                        caption = "Nomor Identitas";
                    }
                    if (row.ColumnName == "dm_nik")
                    {
                        caption = "No ID Debitur";
                    }
                    if (row.ColumnName == "dm_id_pengurus")
                    {
                        caption = "ID Pengurus/Pemilik";
                        width = 200;
                    }
                    if (row.ColumnName == "dm_nama_pengurus")
                    {
                        caption = "Nama Pengurus/Pemilik";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_jumlah_rekening_pelunasan_dipercepat")
                    {
                        caption = "Jumlah Rekening Pelunasan Dipercepat (non-KUR)";
                        width = 350;
                    }
                    if (row.ColumnName == "dm_jumlah_rekening_fasilitas_kredit_baru")
                    {
                        caption = "Jumlah Rekening Fasilitas Kredit Baru (KUR)";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_jml_rek_fasilitas_kredit_baru")
                    {
                        caption = "Jumlah Rekening Fasilitas Kredit Baru (KUR)";
                        width = 300;
                    }
                    if (row.ColumnName == "dm_total_plafon_kredit_baru")
                    {
                        caption = "Total Plafon Kredit Baru (KUR)";
                        width = 200;

                    }
                    if (row.ColumnName == "dm_plafon_kredit_baru")
                    {
                        caption = "Total Plafon Kredit Baru (KUR)";
                        width = 200;

                    }
                    if (row.ColumnName == "dm_total_baki_debet_kredit_baru")
                    {
                        caption = "Total Baki Debet Kredit Baru (KUR)";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_baki_debet_kredit_baru")
                    {
                        caption = "Total Baki Debet Kredit Baru (KUR)";
                        width = 225;
                    }
                    if (row.ColumnName == "dm_no_id_debitur")
                    {
                        caption = "Nomor ID Debitur";
                    }

                    if (row.ColumnName == "dm_jw_macet")
                    {
                        caption = "JW Macet";
                    }
                    if (row.ColumnName == "dm_mulai")
                    {
                        caption = "Tgl Mulai";
                    }
                    if (row.ColumnName == "dm_kondisi")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_no_rekening")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            continue;
                        }
                        else
                        {
                            if (osida.kode == "osida_tanpa_info_keu_mst" || osida.kode == "osida_info_keu_tdk_update_mst" || osida.kode == "osida_keu_buruk_kol_lancar_mst")
                            {
                                caption = "Jumlah Rekening";
                                width = 150;
                            }
                            else if (osida.kode == "osida_agunan_tdk_dinilai_independen_mst" || osida.kode == "osida_kredit_macet_tdk_hb_mst" || osida.kode == "osida_kredit_macet_tdk_hb_det" || osida.kode == "osida_nik_tidak_konsisten_det" || osida.kode == "osida_pemberian_kur_deb_noneligible_det" || osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_det_bu" || osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus")
                            {
                                caption = "Nomor Rekening";
                                width = 225;
                            }
                            else
                            {
                                width = 225;
                            }
                        }

                    }
                    if (row.ColumnName == "dm_baru_perpanjangan")
                    {
                        caption = "Baru/Perpanjangan";
                    }
                    if (row.ColumnName == "dm_kolektibilitas_dpd")
                    {
                        caption = "Kolektibilitas (DPD)";
                    }
                    if (row.ColumnName == "dm_nama_nasabah")
                    {
                        width = 350;
                    }
                    if (row.ColumnName == "dm_jenis_agunan")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_jenis_kredit")
                    {
                        width = 275;
                    }
                    if (row.ColumnName == "dm_bukti_kepemilikan")
                    {
                        width = 300;
                    }
                    if (row.ColumnName == "dm_alamat_agunan")
                    {
                        width = 400;
                    }
                    if (row.ColumnName == "dm_lokasi_agunan")
                    {
                        width = 200;
                    }
                    if (row.ColumnName == "dm_sifat_kredit")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_program_pemerintah")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_jenis_penggunaan")
                    {
                        width = 550;
                    }
                    if (row.ColumnName == "dm_total_tunggakan_bunga_bulan_lalu")
                    {
                        width = 250;
                    }
                    if (row.ColumnName == "dm_plafon_awal")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_baki_debet")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_jml_rekening_baru")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_id_pengurus_pemilik")
                    {
                        if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_total_baki_debet")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst" || osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                        {
                            continue;
                        }
                    }
                    if (row.ColumnName == "dm_total_plafon_awal")
                    {
                        if (osida.kode == "osida_alih_deb_ke_bank_mst")
                        {
                            continue;
                        }
                        if (osida.kode == "osida_potensi_konversi_kur_deb_noneligible_mst")
                        {
                            caption = "Total Baki Debet Bulan Lalu";
                            width = 200;
                        }
                    }
                    if (row.ColumnName == "dm_total_baki_debet_bulan_lalu")
                    {
                        if (osida.kode == "osida_potensi_konversi_kur_deb_noneligible_mst")
                        {
                            caption = "Total Baki Debet Bulan Lalu";
                            width = 200;
                        }
                    }

                    if (row.DataType == "date")
                    {
                        if (row.ColumnName != "dm_periode")
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        {
                            if (isHive == false)
                            {
                                format = "yyyy-MM-dd";
                                colDataType = GridColumnDataType.Date;
                            }
                            else
                            {
                                format = "";
                                colDataType = GridColumnDataType.String;
                            }
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    //if (row.DataType == "decimal")
                    //{
                    //    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)
                    //    .DataType(colDataType).Format(format).CalculateFilterExpression("CFE"));
                    //}
                    //else
                    //{
                    //    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).DataType(colDataType).Format(format));
                    //}
                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).DataType(colDataType).Format(format));
                }
            }
            if (osida.kode == "osida_kredit_macet_tdk_hb_mst")
            {
                grid.Columns(c => c.Add().Caption("Total Nilai Agunan").Columns(c1 =>
                {
                    c1.Add().Caption("NJOP/Nilai Wajar").DataField("dm_njop_nilai_wajar").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Menurut Pelapor").DataField("dm_menurut_pelapor").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Menurut Penilai Independen").DataField("dm_menurut_penilai_independen").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                }));
                grid.Columns(c => c.Add().Caption("Coverage Terhadap Baki Debet").Columns(c1 =>
                {
                    c1.Add().Caption("Menurut Pelapor (%)").DataField("dm_menurut_pelapor_persentase").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                }));
            }
            else if (osida.kode == "osida_agunan_tdk_dinilai_independen_mst")
            {
                grid.Columns(c => c.Add().Caption("Total Nilai Agunan Aset Tetap").Columns(c1 =>
                {
                    c1.Add().Caption("NJOP/Nilai Wajar").DataField("dm_njop_nilai_wajar").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Menurut Pelapor").DataField("dm_menurut_pelapor").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                }));
                grid.Columns(c => c.Add().Caption("Coverage Terhadap Baki Debet (%)").Columns(c1 =>
                {
                    c1.Add().Caption("NJOP/Nilai Wajar (%)").DataField("dm_njop_nilai_wajar_persentase").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Menurut Pelapor (%)").DataField("dm_menurut_pelapor_persentase").Width(375).DataType(GridColumnDataType.Number).Format(",##0");
                }));
            }
            else if (osida.kode == "osida_keu_buruk_kol_lancar_mst")
            {
                grid.Columns(c => c.Add().Caption("Informasi Keuangan Debitur").Columns(c1 =>
                {
                    c1.Add().Caption("Laba/Rugi Bruto 0").DataField("dm_laba_rugi_bruto_0").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Laba/Rugi Tahun Berjalan 0").DataField("dm_laba_rugi_tahun_berjalan_0").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Liabilitas Jangka Pendek 0").DataField("dm_liabilitas_jangka_pendek_0").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Aset Lancar 0").DataField("dm_aset_lancar_0").Width(250).DataType(GridColumnDataType.Number).Format(",##0");

                    c1.Add().Caption("Laba/Rugi Bruto 1").DataField("dm_laba_rugi_bruto_1").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Laba/Rugi Tahun Berjalan 1").DataField("dm_laba_rugi_tahun_berjalan_1").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Liabilitas Jangka Pendek 1").DataField("dm_liabilitas_jangka_pendek_1").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Aset Lancar 1").DataField("dm_aset_lancar_1").Width(250).DataType(GridColumnDataType.Number).Format(",##0");

                    c1.Add().Caption("Laba/Rugi Bruto 2").DataField("dm_laba_rugi_bruto_2").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Laba/Rugi Tahun Berjalan 2").DataField("dm_laba_rugi_tahun_berjalan_2").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Liabilitas Jangka Pendek 2").DataField("dm_liabilitas_jangka_pendek_2").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Aset Lancar 2").DataField("dm_aset_lancar_2").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                }));
            }
            else if (osida.kode == "osida_alih_deb_ke_bank_mst")
            {
                grid.Columns(c => c.Add().Caption("Data 6 Bulan Kebelakang yang Mengalami Pengalihan").Columns(c1 =>
                {
                    c1.Add().Caption("Jumlah Rekening").DataField("dm_no_rekening").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Total Plafon Awal").DataField("dm_total_plafon_awal").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Total Baki Debet").DataField("dm_total_baki_debet").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Total Tunggakan (Pokok + Bunga)").DataField("dm_tunggakan").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                }));
                grid.Columns(c => c.Add().Caption("Data Bulan Laporan").Columns(c1 =>
                {
                    c1.Add().Caption("Jml Rekening Baru").DataField("dm_jml_rekening_baru").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Plafon Awal").DataField("dm_plafon_awal").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Baki Debet").DataField("dm_baki_debet").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                }));
            }
            else if (osida.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
            {
                grid.Columns(c => c.Add().Caption("Nasabah").VisibleIndex(6).Columns(c1 =>
                {
                    c1.Add().Caption("Kualitas").DataField("dm_kualitas").Width(250);
                    c1.Add().Caption("Total Plafon").DataField("dm_total_plafon").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Total Baki Debet").DataField("dm_total_baki_debet").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                }));
                grid.Columns(c => c.Add().Caption("Nama Pengurus/Pemilik").DataField("dm_nama_pengurus_pemilik").Width(250).DataType(GridColumnDataType.String).VisibleIndex(7));
                grid.Columns(c => c.Add().Caption("Pengurus/Pemilik").VisibleIndex(8).Columns(c1 =>
                {
                    c1.Add().Caption("Jabatan").DataField("dm_jabatan").Width(250);
                    c1.Add().Caption("Pangsa Kepemilikan").DataField("dm_pangsa_pemilik").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Total Baki Debet Bermasalah").DataField("dm_total_baki_debet_bermasalah").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Total Tunggakan Bunga Bermasalah").DataField("dm_total_tunggakan_bunga").Width(250).DataType(GridColumnDataType.Number).Format(",##0");
                }));
            }
            return grid;
        }

        public static DataGridBuilder<T> MonitoringDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var list = db.vw_TableDictionary.Where(x => x.TableName == "log_monitoring_bda_slik_det").ToList();

            foreach (var row in list)
            {
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");

                if (row.ColumnName != "rowid" && row.ColumnName != "dm_outstanding" && row.ColumnName != "dm_pyearmonth" && row.ColumnName != "row_id")
                {

                    var width = 150;
                    var format = "";
                    var colDataType = GridColumnDataType.String;

                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;

                    }

                    if (row.DataType == "date")
                    {
                        if (row.ColumnName != "bulan_laporan")
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        {
                            if (isHive == false)
                            {
                                format = "yyyy-MM-dd";
                                colDataType = GridColumnDataType.Date;
                            }
                            else
                            {
                                format = "";
                                colDataType = GridColumnDataType.String;
                            }
                        }

                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;

                    }

                    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).DataType(colDataType).Format(format));

                }
            }
            return grid;
        }

        public static DataGridBuilder<T> IPDataGrid<T>(this DataGridBuilder<T> grid, DataEntities db, string kode, bool isHive)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
            var regex = new Regex(@"\Aip_relation");
            var regexRel = new Regex(@"\Arelation|\Asuspect");

            //grid.Columns(c => c.Add().Caption("No").Width(50).Visible(true).AllowFiltering(false).CellTemplate(new JS("GetRowNumber")));
            grid.Columns(c => c.Add().Caption("No").Width(50).Visible(true).AllowFiltering(false).DataField("no"));

            /*
            grid.CustomUnboundColumnData += (sender, e) =>
            {
                if (e.Column.FieldName == "No")
                {
                    e.DisplayText = e.RowHandle.ToString();                    
                }
            };
            */


            var list = db.vw_TableDictionary.Where(x => x.TableName == kode).ToList();

            foreach (var row in list)
            {
                //string left7 = "";
                bool isInput = false;
                bool allowFilter = true;
                var caption = cultInfo.ToTitleCase(row.ColumnName.Replace("dm_", "").Replace("_", " "));
                caption = caption.Replace("Ljk", "LJK");
                caption = caption.Replace("Cif", "CIF");
                caption = caption.Replace("Njop", "NJOP");


                if (row.ColumnName != "rowid" && row.ColumnName != "etl_date")
                {
                    var width = 150;
                    var format = "";
                    var visible = false;
                    var colDataType = GridColumnDataType.String;


                    if (row.DataType == "date")
                    {
                        if (row.ColumnName != "dm_periode")
                        {
                            format = "yyyy-MM-dd";
                            colDataType = GridColumnDataType.Date;
                        }
                        else
                        {
                            if (isHive == false)
                            {
                                format = "yyyy-MM-dd";
                                colDataType = GridColumnDataType.Date;
                            }
                            else
                            {
                                format = "";
                                colDataType = GridColumnDataType.String;
                            }
                            continue;
                        }
                    }
                    else if (row.DataType == "int" || row.DataType == "decimal" || row.DataType == "bigint")
                    {
                        format = ",##0";
                        colDataType = GridColumnDataType.Number;
                    }

                    if (row.ColumnName.Contains("tanggal"))
                    {
                        width = 130;
                    }

                    if (row.ColumnName == "no")
                    {
                        caption = "No";
                        width = 50;
                        visible = true;
                    }

                    if (row.ColumnName == "sid")
                    {
                        caption = "Nomor SID";
                        width = 130;
                        visible = true;
                    }

                    if (row.ColumnName == "ktp") caption = "Nomor KTP";
                    if (row.ColumnName == "npwp") caption = "Nomor NPWP";
                    if (row.ColumnName == "trade_id") caption = "Trading ID";


                    if (row.ColumnName == "nama_sid")
                    {
                        caption = "Nama SID";
                        width = 300;
                        visible = true;
                        allowFilter = false;
                    }

                    if (row.ColumnName == "accountbalancestatuscode") caption = "Balance Status";
                    if (row.ColumnName == "rekening_status") caption = "Account Status";
                    if (row.ColumnName == "securityname") caption = "Nama Efek";
                    if (row.ColumnName == "securitycode") caption = "Kode Efek";

                    if (kode == "ip_sid")
                    {
                        //if (new string[] { "", "" }.Any(row.ColumnName.Contains))
                        if ((new string[] { "trade_id", "ktp", "npwp" }.Any(s => row.ColumnName == s)))
                        {
                            visible = true;
                        }

                        if (row.ColumnName == "gender")
                        {
                            caption = "Jenis Kelamin";
                            width = 250;
                        }
                        //else continue;

                    }
                    else if (kode == "ip_transaction")
                    {
                        visible = true;
                        string left3 = row.ColumnName.Substring(0, 3);
                        if (left3 == "buy" || left3 == "sel" || left3 == "net") continue;
                        if ((new string[] { "trade_id", "ktp", "npwp", "periode", "system" }.Any(s => row.ColumnName == s))) visible = false;
                        
                    }
                    else if (kode == "ip_ownership")
                    {
                        visible = true;
                        if ((new string[] { "trade_id", "ktp", "npwp", "periode" }.Any(s => row.ColumnName == s))) visible = false;
                        
                    }
                    else if (regex.Match(kode).Success)
                    {
                        //left7 = row.ColumnName.Length >= 7 ? row.ColumnName.Substring(0, 7) : "";
                        if (regexRel.Match(row.ColumnName).Success) isInput = true;
                        if (row.ColumnName == "isdirect") caption = "Direct / Indirect";
                        else if (row.ColumnName == "noterelationship") caption = "Keterangan Keterkaitan";
                        else if (row.ColumnName == "suspectnote1") caption = "Keterangan Suspect";
                        else if (row.ColumnName == "suspectnote2") caption = "Keterangan Suspect";
                        else if (row.ColumnName == "suspectnote3") caption = "Keterangan Suspect";
                        else if (row.ColumnName == "relationshipperiod") caption = "Periode Keterkaitan (Pemeriksaan)";
                        else if (row.ColumnName == "relationshipgroup") caption = "Grup Keterkaitan";
                        else if (row.ColumnName == "similaritynote") caption = "Keterangan Kemiripan";
                        else if (row.ColumnName == "quantity") caption = "Volume";

                        if (kode == "ip_relation_transaction")
                        {
                            string left3 = row.ColumnName.Substring(0, 3);
                            if (left3 == "buy" || left3 == "sel" || left3 == "net") continue;

                            if (row.ColumnName == "similaritynote")

                            {
                                grid.Columns(c => c.Add().Caption("Buy").CssClass("header-buy").Columns(c1 =>
                                {
                                    c1.Add().Caption("Volume").DataField("buy_quantity").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                                    c1.Add().Caption("Value").DataField("buy_value").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                                    c1.Add().Caption("Transaksi").DataField("buy_freq").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                                }));
                                grid.Columns(c => c.Add().Caption("Sell").CssClass("header-sell").Columns(c1 =>
                                {
                                    c1.Add().Caption("Direct/Indirect").DataField("sell_isdirect").Width(150).DataType(GridColumnDataType.String);
                                    c1.Add().Caption("Keterangan Keterkaitan").DataField("sell_similaritynote").Width(150).DataType(GridColumnDataType.String);
                                    c1.Add().Caption("Keterangan Suspect").DataField("sell_suspectnote").Width(150).DataType(GridColumnDataType.String);
                                }));
                                grid.Columns(c => c.Add().Caption("Net Sell/Buy").CssClass("header-net").Columns(c1 =>
                                {
                                    c1.Add().Caption("Keterangan Suspect").DataField("net_suspectnote1").Width(150).DataType(GridColumnDataType.String);
                                    c1.Add().Caption("Keterangan Suspect").DataField("net_suspectnote2").Width(150).DataType(GridColumnDataType.String);
                                    c1.Add().Caption("Periode Keterkaitan/Pemeriksaan").DataField("net_relationshipperiod").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                                }));
                            }
                        }

                        visible = true;
                    }
                    else
                    {
                        visible = true;
                    }
                    //if (row.DataType == "decimal")
                    //{
                    //    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width)
                    //    .DataType(colDataType).Format(format).CalculateFilterExpression("CFE"));
                    //}
                    //else
                    //{
                    //    grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).DataType(colDataType).Format(format));
                    //}

                    //if ((left7 == "relatio") || (left7 == "suspect"))
                    if (isInput)
                        grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).Visible(visible).DataType(colDataType).Format(format).CssClass("header-green"));
                    else
                    {
                        if (kode != "ip_sid" && row.ColumnName == "sid")
                        {
                            string ct = "<text><a href=\"ip_sid?detailsid=<%- data.lem %>\"><%- value %></a></text>";
                            grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).Visible(visible).DataType(colDataType).Format(format).CellTemplate(ct));

                        }
                        else if (row.ColumnName == "trade_id")
                        {
                            grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).Visible(visible).DataType(colDataType).Format(format).AllowFiltering(allowFilter).VisibleIndex(2));
                        }
                        else
                            grid.Columns(c => c.Add().Caption(caption).DataField(row.ColumnName).Width(width).Visible(visible).DataType(colDataType).Format(format).AllowFiltering(allowFilter));
                    }
                }
            }

            if (kode == "ip_transaction")
            {
                grid.Columns(c => c.Add().Caption("Buy").CssClass("header-buy").Columns(c1 =>
                {
                    c1.Add().Caption("Value").DataField("buy_value").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Volume").DataField("buy_quantity").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Transaksi").DataField("buy_freq").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                }));
                grid.Columns(c => c.Add().Caption("Sell").CssClass("header-sell").Columns(c1 =>
                {
                    c1.Add().Caption("Value").DataField("sell_value").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Volume").DataField("sell_quantity").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Transaksi").DataField("sell_freq").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                }));
                grid.Columns(c => c.Add().Caption("Net Sell/Buy").CssClass("header-net").Columns(c1 =>
                {
                    c1.Add().Caption("Value").DataField("net_value").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Volume").DataField("net_quantity").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                    c1.Add().Caption("Transaksi").DataField("net_freq").Width(150).DataType(GridColumnDataType.Number).Format(",##0");
                }));
            }

            if (regex.Match(kode).Success)
            {
                //grid.Columns(c => c.Add().Caption("Aksi").Width(100).Type(GridCommandColumnType.Buttons).Buttons(b => { b.Add().Icon("icon_here").OnClick(@< text > jsFunctionToRedirectPage </ text >)}));//.DataType(colDataType);
                grid.Editing(e => e.UseIcons(true)
                                    .Mode(GridEditMode.Popup).AllowUpdating(true)
                                    .Popup(p => p.Title("Keterangan").ShowTitle(true).Width(700).Height(525)
                                    .ToolbarItems(items =>
                                    {
                                        items.Add().Toolbar(Toolbar.Bottom).Location(ToolbarItemLocation.After)
                                            .Widget(w => w.Button()
                                                .Type(ButtonType.Success)
                                                .StylingMode(ButtonStylingMode.Outlined)
                                                .Text("Simpan")
                                                .OnClick("function() { $('#gridPopup').dxDataGrid('instance').saveEditData(); }")
                                        );
                                        items.Add().Toolbar(Toolbar.Bottom).Location(ToolbarItemLocation.After)
                                            .Widget(w => w.Button()
                                                .Type(ButtonType.Danger)
                                                .StylingMode(ButtonStylingMode.Outlined)
                                                .Text("Batal")
                                        //.OnClick("function() { $('#gridPopup').dxDataGrid('instance').cancelEditData(); }")
                                        );
                                        //items.Add().Toolbar(Toolbar.Bottom).Location(ToolbarItemLocation.Before)
                                        //    .Widget(w => w.Button()
                                        //        .StylingMode(ButtonStylingMode.Outlined)
                                        //        .Text("Copy Data")
                                        //        .OnClick("() => copyDataClick('gridPopup')")
                                        //);
                                    }))
                                    .Form(f => f.Items(items =>
                                    {
                                        items.AddGroup().ColCount(2).ColSpan(2).Items(groupItems =>
                                        {
                                            if (kode == "ip_relation_sid")
                                            {
                                                groupItems.AddSimple().DataField("suspectnote1").ColSpan(2).Editor(editor => editor.TextArea().Height(100)).Name("suspectnote1");
                                                groupItems.AddSimple().DataField("suspectnote2").ColSpan(2).Editor(editor => editor.TextArea().Height(100)).Name("suspectnote2");
                                                groupItems.AddSimple().DataField("suspectnote3").ColSpan(2).Editor(editor => editor.TextArea().Height(100)).Name("suspectnote3");
                                            }
                                            groupItems.AddSimple().DataField("relationshipperiod").Name("relationshipperiod");
                                            groupItems.AddSimple().DataField("relationshipgroup").Name("relationshipgroup");
                                        });


                                    }))
                );
            }

            if (kode != "ip_sid")
            {
                grid.OnRowDblClick("onRowDblClick");
            }

            return grid;
        }

    }
}
