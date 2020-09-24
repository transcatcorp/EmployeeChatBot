CREATE PROCEDURE [dbo].[Report_Save]
	@ReportId int,
	@Cough bit,
	@Fever bit,
	@Breathing bit,
	@SoreThroat bit,
	@BodyAches bit,
	@LossOfSmell bit,
	@VomitDiarrhea bit,
	@Traveled bit,
	@CloseProximity bit
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	
	UPDATE dbo.Report
		SET Fever = @Fever,
			Breathing = @Breathing,
			Coughing = @Cough,
			SoreThroat = @SoreThroat,
			BodyAches = @BodyAches,
			LossOfTasteSmell = @LossOfSmell,
			Traveled = @Traveled,
			VomitDiarrhea = @VomitDiarrhea,
			CloseProximity = @CloseProximity,
			CompletedAt = GETDATE()
	WHERE Report_ID = @ReportId
		AND CompletedAt IS NULL

END