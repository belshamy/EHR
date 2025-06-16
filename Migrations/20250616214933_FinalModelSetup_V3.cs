using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHRsystem.Migrations
{
    /// <inheritdoc />
    public partial class FinalModelSetup_V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId1",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnosis_Doctors_DoctorId",
                table: "Diagnosis");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnosis_Patients_PatientId",
                table: "Diagnosis");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResult_AspNetUsers_ApplicationUserId",
                table: "LabResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResult_Doctors_OrderedByDoctorId",
                table: "LabResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResult_LabTests_LabTestId",
                table: "LabResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResult_Patients_PatientId",
                table: "LabResult");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalFile_Patients_PatientId",
                table: "MedicalFile");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistory_Patients_PatientId",
                table: "MedicalHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorId1",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Patients_PatientId1",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Doctors_DoctorId1",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Patients_PatientId1",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Doctors_DoctorId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Patients_PatientId1",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_DoctorId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_PatientId1",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_DoctorId1",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_PatientId1",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_DoctorId1",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_PatientId1",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId1",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistory",
                table: "MedicalHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalFile",
                table: "MedicalFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabResult",
                table: "LabResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagnosis",
                table: "Diagnosis");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "MedicalHistory",
                newName: "MedicalHistories");

            migrationBuilder.RenameTable(
                name: "MedicalFile",
                newName: "MedicalFiles");

            migrationBuilder.RenameTable(
                name: "LabResult",
                newName: "LabResults");

            migrationBuilder.RenameTable(
                name: "Diagnosis",
                newName: "Diagnoses");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistory_PatientId",
                table: "MedicalHistories",
                newName: "IX_MedicalHistories_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalFile_PatientId",
                table: "MedicalFiles",
                newName: "IX_MedicalFiles_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResult_PatientId",
                table: "LabResults",
                newName: "IX_LabResults_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResult_OrderedByDoctorId",
                table: "LabResults",
                newName: "IX_LabResults_OrderedByDoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResult_LabTestId",
                table: "LabResults",
                newName: "IX_LabResults_LabTestId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResult_ApplicationUserId",
                table: "LabResults",
                newName: "IX_LabResults_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnosis_PatientId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnosis_DoctorId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalFiles",
                table: "MedicalFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabResults",
                table: "LabResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagnoses",
                table: "Diagnoses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Doctors_DoctorId",
                table: "Diagnoses",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Patients_PatientId",
                table: "Diagnoses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResults_AspNetUsers_ApplicationUserId",
                table: "LabResults",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LabResults_Doctors_OrderedByDoctorId",
                table: "LabResults",
                column: "OrderedByDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResults_LabTests_LabTestId",
                table: "LabResults",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResults_Patients_PatientId",
                table: "LabResults",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalFiles_Patients_PatientId",
                table: "MedicalFiles",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistories_Patients_PatientId",
                table: "MedicalHistories",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Doctors_DoctorId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Patients_PatientId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResults_AspNetUsers_ApplicationUserId",
                table: "LabResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResults_Doctors_OrderedByDoctorId",
                table: "LabResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResults_LabTests_LabTestId",
                table: "LabResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResults_Patients_PatientId",
                table: "LabResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalFiles_Patients_PatientId",
                table: "MedicalFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistories_Patients_PatientId",
                table: "MedicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalFiles",
                table: "MedicalFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabResults",
                table: "LabResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagnoses",
                table: "Diagnoses");

            migrationBuilder.RenameTable(
                name: "MedicalHistories",
                newName: "MedicalHistory");

            migrationBuilder.RenameTable(
                name: "MedicalFiles",
                newName: "MedicalFile");

            migrationBuilder.RenameTable(
                name: "LabResults",
                newName: "LabResult");

            migrationBuilder.RenameTable(
                name: "Diagnoses",
                newName: "Diagnosis");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistories_PatientId",
                table: "MedicalHistory",
                newName: "IX_MedicalHistory_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalFiles_PatientId",
                table: "MedicalFile",
                newName: "IX_MedicalFile_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResults_PatientId",
                table: "LabResult",
                newName: "IX_LabResult_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResults_OrderedByDoctorId",
                table: "LabResult",
                newName: "IX_LabResult_OrderedByDoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResults_LabTestId",
                table: "LabResult",
                newName: "IX_LabResult_LabTestId");

            migrationBuilder.RenameIndex(
                name: "IX_LabResults_ApplicationUserId",
                table: "LabResult",
                newName: "IX_LabResult_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_PatientId",
                table: "Diagnosis",
                newName: "IX_Diagnosis_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_DoctorId",
                table: "Diagnosis",
                newName: "IX_Diagnosis_DoctorId");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "TestResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "TestResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                table: "Prescriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "Prescriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistory",
                table: "MedicalHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalFile",
                table: "MedicalFile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabResult",
                table: "LabResult",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagnosis",
                table: "Diagnosis",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_DoctorId",
                table: "TestResults",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_PatientId1",
                table: "TestResults",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_DoctorId1",
                table: "Prescriptions",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId1",
                table: "Prescriptions",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId1",
                table: "MedicalRecords",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId1",
                table: "MedicalRecords",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId1",
                table: "Appointments",
                column: "PatientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId1",
                table: "Appointments",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnosis_Doctors_DoctorId",
                table: "Diagnosis",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnosis_Patients_PatientId",
                table: "Diagnosis",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResult_AspNetUsers_ApplicationUserId",
                table: "LabResult",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LabResult_Doctors_OrderedByDoctorId",
                table: "LabResult",
                column: "OrderedByDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResult_LabTests_LabTestId",
                table: "LabResult",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResult_Patients_PatientId",
                table: "LabResult",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalFile_Patients_PatientId",
                table: "MedicalFile",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistory_Patients_PatientId",
                table: "MedicalHistory",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorId1",
                table: "MedicalRecords",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Patients_PatientId1",
                table: "MedicalRecords",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Doctors_DoctorId1",
                table: "Prescriptions",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Patients_PatientId1",
                table: "Prescriptions",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Doctors_DoctorId",
                table: "TestResults",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Patients_PatientId1",
                table: "TestResults",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }
    }
}
