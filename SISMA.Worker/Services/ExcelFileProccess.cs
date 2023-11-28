using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SISMA.Core.Contracts;
using SISMA.Core.Services;
using SISMA.Infrastructure.Contracts.Data;
using SISMA.Infrastructure.Data.Common;
using SISMA.Worker.Contracts;
using System.Xml.Serialization;

namespace SISMA.Worker.Services
{
    public class ExcelFileProccess : BaseService, IExcelFileProccess
    {
        private readonly IDataService dataService;


        public ExcelFileProccess(IRepository _repo,
             ILogger<ExcelFileProccess> _logger,
            IDataService _dataService)
        {
            repo = _repo;
            logger = _logger;
            dataService = _dataService;
        }


        public async Task<bool> ProcessAsync(string fileName, byte[] fileContent)
        {
            if (fileName.EndsWith("xml"))
            {
                return await processXml(fileName, fileContent);
            }

            ExcelSheetData = CreateExcelSheetObjectFromByteArray(fileName, fileContent);
            if (ExcelSheetData == null)
            {
                return false;
            }
            try
            {
                var model = mapExcelToModel(fileName);
                var importResult = await dataService.SaveData(model);
                return importResult.IsSuccessfull;
            }
            catch(Exception ex)
            {
                return false;
            }
          
        }

        private async Task<bool> processXml(string fileName, byte[] fileContent)
        {
            string operationXML = System.Text.Encoding.UTF8.GetString(fileContent);
            var serializer = new XmlSerializer(typeof(SismaModel));
            SismaModel? model;

            try
            {
                using (TextReader reader = new StringReader(operationXML))
                {
                    model = (SismaModel?)serializer.Deserialize(reader);
                }

                if (model != null)
                {
                    model.Context.FromFTP = true;
                    var importResult = await dataService.SaveData(model);
                    return importResult.IsSuccessfull;
                }

                return false;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error deserializing {fileName}; Content length: {fileContent.Length}");
                return false;
            }
        }


        SismaModel mapExcelToModel(string fileName)
        {
            SismaModel model = new SismaModel();
            int maxCol = getMaxCol();
            //int maxRow = ExcelSheetData.LastRowNum;// getMaxRow();
            int maxRow = getMaxRow();

            model.Context = new SismaContextModel()
            {
                IntegrationType = fileName.Substring(6, 1),
                ReportType = fileName.Substring(6, 4),
                PeriodYear = int.Parse(fileName.Substring(11, 4)),
                PeriodNumber = int.Parse(fileName.Substring(16, 2)),
                MethodName = SismaConstants.Methods.Add,
                FromFTP = true
            };

            List<SismaCodeModel> codes = new List<SismaCodeModel>();
            //ИБД кодовете
            for (int col = 2; col <= maxCol; col++)
            {
                var codeModel = new SismaCodeModel();
                codeModel.IbdCode = GetCellDataFromISheet_AsString(0, col);
                codeModel.Count = safeIntFromCell(1, col);

                List<SismaCodeDetailModel> details = new List<SismaCodeDetailModel>();

                for (int row = 2; row <= maxRow; row++)
                {
                    var detail = new SismaCodeDetailModel();
                    detail.EntityCode = GetCellDataFromISheet_AsString(row, 0);
                    if (string.IsNullOrEmpty(detail.EntityCode))
                    {
                        continue;
                    }
                    detail.Count = safeIntFromCell(row, col);
                    details.Add(detail);
                }
                codeModel.Details = details.ToArray();

                codes.Add(codeModel);
            }
            model.Codes = codes.ToArray();

            return model;
        }

        #region Base Excel methods
        ISheet? ExcelSheetData;

        int safeIntFromCell(int row, int col)
        {
            var textValue = GetCellDataFromISheet_AsString(row, col);
            if (!string.IsNullOrEmpty(textValue))
            {
                return int.Parse(textValue);
            }
            return 0;
        }
        int getMaxCol()
        {
            int pos = 2;
            while (true)
            {
                if (string.IsNullOrEmpty(GetCellDataFromISheet_AsString(0, pos)))
                {
                    return pos - 1;
                }
                pos++;
            }
        }

        int getMaxRow()
        {
            int pos = 2;
            while (true)
            {
                if (string.IsNullOrEmpty(GetCellDataFromISheet_AsString(pos, 0)))
                {
                    return pos - 1;
                }
                pos++;
            }
        }


        ISheet? CreateExcelSheetObjectFromByteArray(string fileName, byte[] fileContent)
        {
            ISheet sheet;
            using (var stream = new MemoryStream(fileContent))
            {
                try
                {
                    if (Path.GetExtension(fileName) == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0);
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"Invalid file {fileName}");
                    return null;
                }
            }

            return sheet;
        }

        /// <summary>
        /// Хвърля ProcessException!!!
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="IsHaveMergedCell"></param>
        /// <returns></returns>
        protected string GetCellDataFromISheet_AsString(int Row, int Column, bool IsHaveMergedCell = false)
        {
            var value = GetCellDataFromISheet(Row, Column, IsHaveMergedCell);
            if (value != null)
            {
                return value.ToString();
            }
            else
            {
                return string.Empty;
            }

            //return GetCellDataFromISheet(Row, Column, IsHaveMergedCell).ToString();
        }

        /// <summary>
        /// Хвърля ProcessException!!!
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="IsHaveMergedCell"></param>
        /// <returns></returns>
        object? GetCellDataFromISheet(int Row, int Column, bool IsHaveMergedCell = false)
        {
            object? result = null;

            try
            {
                // Изчитане на данни от клетка ПРИ наличие на Merge-нати клетки
                if (IsHaveMergedCell)
                {
                    IRow row = this.ExcelSheetData.GetRow(Row);
                    ICell cellBeforeTestMerged = row.GetCell(Column);
                    ICell cell = GetFirstCellInMergedRegionContainingCell(cellBeforeTestMerged);
                    result = GetCellValue(cell ?? cellBeforeTestMerged);
                }
                else // Изчитане на данни от клетка БЕЗ наличие на Merge-нати клетки
                {
                    IRow row = this.ExcelSheetData.GetRow(Row);
                    if (row == null)
                    {
                        return null;
                    }
                    ICell cell = row.GetCell(Column);
                    if (cell == null)
                    {
                        return null;
                    }
                    result = GetCellValue(cell);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, @"GetCellDataFromISheet");
                throw new Exception($"Invalid value at row {Row}, col {Column}");
            }

            return result;
        }

        private ICell GetFirstCellInMergedRegionContainingCell(ICell cell)
        {
            if (cell != null && cell.IsMergedCell)
            {
                ISheet sheet = cell.Sheet;
                for (int i = 0; i < sheet.NumMergedRegions; i++)
                {
                    CellRangeAddress region = sheet.GetMergedRegion(i);

                    if ((region.FirstRow <= cell.RowIndex && region.LastRow >= cell.RowIndex)
                     && (region.FirstColumn <= cell.ColumnIndex && region.LastColumn >= cell.ColumnIndex))
                    {
                        IRow row = sheet.GetRow(region.FirstRow);
                        ICell firstCell = row?.GetCell(region.FirstColumn);
                        return firstCell;
                    }
                }
                return null;
            }
            return cell;
        }


        Object GetCellValue(ICell cell)
        {
            Object result = null;
            CellType _CellType;

            if (cell == null) return result;

            if (cell.CellType == CellType.Formula)
                _CellType = cell.CachedFormulaResultType;
            else
                _CellType = cell.CellType;

            switch (_CellType)
            {
                case CellType.Blank:
                    result = cell.StringCellValue;
                    result = null;
                    break;

                case CellType.Boolean:
                    result = cell.BooleanCellValue;
                    break;

                case CellType.Error:
                    //result = cell.ErrorCellValue;
                    result = "ERROR";
                    break;

                case CellType.Numeric:
                    if (HSSFDateUtil.IsCellDateFormatted(cell))
                        result = cell.DateCellValue;
                    else
                        result = cell.NumericCellValue;
                    break;

                case CellType.String:
                    result = cell.StringCellValue;
                    break;

                case CellType.Unknown:
                    result = "UNKNOWN";
                    break;

            }
            return result;
            //return result.ToString();
        }

        #endregion
    }
}
