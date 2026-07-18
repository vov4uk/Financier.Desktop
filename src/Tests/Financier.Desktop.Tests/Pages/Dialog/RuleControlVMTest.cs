namespace Financier.Desktop.Tests.Pages.Dialog
{
    using System.Collections.Generic;
    using System.Linq;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Converters;
    using Financier.Desktop.Data;
    using Financier.Desktop.Pages.Dialogs;
    using Xunit;

    public class RuleControlVMTest
    {
        // ── helpers ──────────────────────────────────────────────────────────

        private static RuleDto MccRule(
            Mcc mcc,
            int? payeeId = 1,
            int? locationId = null,
            int? categoryId = null,
            int? projectId = null) =>
            new RuleDto
            {
                Condition = RuleConditionType.MCC,
                MCCCategory = mcc,
                PayeeId = payeeId,
                LocationId = locationId,
                CategoryId = categoryId,
                ProjectId = projectId,
            };

        private static RuleDto DescriptionRule(
            string description,
            RuleConditionType condition = RuleConditionType.DescriptionContains,
            int? payeeId = 1,
            int? locationId = null,
            int? categoryId = null,
            int? projectId = null) =>
            new RuleDto
            {
                Condition = condition,
                MCCCategory = Mcc.none,
                Description = description,
                PayeeId = payeeId,
                LocationId = locationId,
                CategoryId = categoryId,
                ProjectId = projectId,
            };

        // ── Constructor ──────────────────────────────────────────────────────

        [Fact]
        public void Constructor_SetsEntityProperty()
        {
            var entity = DescriptionRule("Test");
            var vm = new RuleControlVM(entity);
            Assert.Same(entity, vm.Entity);
        }

        [Fact]
        public void Constructor_SelectedConditionType_MatchesEntityCondition()
        {
            var entity = MccRule(Mcc.accessories);
            var vm = new RuleControlVM(entity);
            Assert.Equal(RuleConditionType.MCC, vm.SelectedConditionType);
        }

        [Fact]
        public void Constructor_SelectedMccTitle_MatchesEntityMCCCategory()
        {
            var entity = MccRule(Mcc.accessories);
            var vm = new RuleControlVM(entity);
            Assert.Equal(Mcc.accessories.GetEnumLocalizedMccDescription(), vm.SelectedMccTitle);
        }

        [Fact]
        public void Constructor_MccTitles_IsPopulatedFromDbManual()
        {
            var vm = new RuleControlVM(DescriptionRule("Test"));
            Assert.NotEmpty(vm.MccTitles);
            Assert.Equal(DbManual.MCCTitles.Count, vm.MccTitles.Count);
        }

        [Fact]
        public void Constructor_MccTitles_IsAlphabeticallyOrdered()
        {
            var vm = new RuleControlVM(DescriptionRule("Test"));
            Assert.Equal(vm.MccTitles.OrderBy(x => x).ToList(), vm.MccTitles);
        }

        // ── IsMCCSelected ────────────────────────────────────────────────────

        [Theory]
        [InlineData(RuleConditionType.MCC, true)]
        [InlineData(RuleConditionType.DescriptionContains, false)]
        [InlineData(RuleConditionType.DescriptionMatches, false)]
        public void IsMCCSelected_ReturnsExpectedValue_ForConditionType(RuleConditionType conditionType, bool expected)
        {
            var entity = DescriptionRule("Test", condition: conditionType);
            var vm = new RuleControlVM(entity);
            Assert.Equal(expected, vm.IsMCCSelected);
        }

        // ── SelectedConditionType setter ─────────────────────────────────────

        [Fact]
        public void SelectedConditionType_WhenChanged_RaisesPropertyChangedForSelfAndIsMCCSelected()
        {
            var entity = DescriptionRule("Test"); // starts as DescriptionContains
            var vm = new RuleControlVM(entity);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedConditionType = RuleConditionType.MCC;

            Assert.Contains(nameof(vm.SelectedConditionType), raised);
            Assert.Contains(nameof(vm.IsMCCSelected), raised);
        }

        [Fact]
        public void SelectedConditionType_WhenSetToSameValue_DoesNotRaisePropertyChanged()
        {
            var entity = DescriptionRule("Test"); // Condition = DescriptionContains
            var vm = new RuleControlVM(entity);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedConditionType = RuleConditionType.DescriptionContains; // same value

            Assert.Empty(raised);
        }

        // ── SelectedMccTitle setter ──────────────────────────────────────────

        [Fact]
        public void SelectedMccTitle_WhenChanged_RaisesPropertyChanged()
        {
            var entity = MccRule(Mcc.none);
            var vm = new RuleControlVM(entity);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedMccTitle = Mcc.accessories.GetEnumLocalizedMccDescription();

            Assert.Contains(nameof(vm.SelectedMccTitle), raised);
        }

        [Fact]
        public void SelectedMccTitle_WhenSetToSameValue_DoesNotRaisePropertyChanged()
        {
            var entity = MccRule(Mcc.accessories);
            var vm = new RuleControlVM(entity);
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedMccTitle = Mcc.accessories.GetEnumLocalizedMccDescription(); // same as ctor

            Assert.Empty(raised);
        }

        // ── CanSaveCommandExecute – MCC condition ────────────────────────────

        [Fact]
        public void SaveCommand_CanExecute_MCC_ValidTitle_WithPayeeId_ReturnsTrue()
        {
            var vm = new RuleControlVM(MccRule(Mcc.accessories, payeeId: 1));
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_ValidTitle_WithLocationId_ReturnsTrue()
        {
            var vm = new RuleControlVM(MccRule(Mcc.accessories, payeeId: null, locationId: 1));
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_ValidTitle_WithCategoryId_ReturnsTrue()
        {
            var vm = new RuleControlVM(MccRule(Mcc.accessories, payeeId: null, categoryId: 1));
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_ValidTitle_WithProjectId_ReturnsTrue()
        {
            var vm = new RuleControlVM(MccRule(Mcc.accessories, payeeId: null, projectId: 1));
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_NoneMccTitle_ReturnsFalse()
        {
            var vm = new RuleControlVM(MccRule(Mcc.none, payeeId: 1));
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_NullTitle_ReturnsFalse()
        {
            var vm = new RuleControlVM(MccRule(Mcc.accessories, payeeId: 1));
            vm.SelectedMccTitle = null;
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_UnknownTitle_ReturnsFalse()
        {
            var vm = new RuleControlVM(MccRule(Mcc.accessories, payeeId: 1));
            vm.SelectedMccTitle = "not_a_valid_mcc_title";
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_MCC_ValidTitle_NoAccountIds_ReturnsFalse()
        {
            var vm = new RuleControlVM(
                MccRule(Mcc.accessories, payeeId: null, locationId: null, categoryId: null, projectId: null));
            Assert.False(vm.SaveCommand.CanExecute());
        }

        // ── CanSaveCommandExecute – non-MCC conditions ───────────────────────

        [Fact]
        public void SaveCommand_CanExecute_DescriptionContains_WithDescription_WithPayeeId_ReturnsTrue()
        {
            var vm = new RuleControlVM(DescriptionRule("Google", payeeId: 1));
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_DescriptionMatches_WithDescription_WithLocationId_ReturnsTrue()
        {
            var vm = new RuleControlVM(
                DescriptionRule("Google", condition: RuleConditionType.DescriptionMatches, payeeId: null, locationId: 1));
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_Description_NullDescription_ReturnsFalse()
        {
            var vm = new RuleControlVM(DescriptionRule(null, payeeId: 1));
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_Description_EmptyDescription_ReturnsFalse()
        {
            var vm = new RuleControlVM(DescriptionRule(string.Empty, payeeId: 1));
            Assert.False(vm.SaveCommand.CanExecute());
        }

        [Fact]
        public void SaveCommand_CanExecute_Description_WithDescription_NoAccountIds_ReturnsFalse()
        {
            var vm = new RuleControlVM(
                DescriptionRule("Google", payeeId: null, locationId: null, categoryId: null, projectId: null));
            Assert.False(vm.SaveCommand.CanExecute());
        }

        // ── OnRequestSave ────────────────────────────────────────────────────

        [Fact]
        public void OnRequestSave_MCC_SetsCondition_ClearsDescription_SetsMCCCategory()
        {
            var entity = MccRule(Mcc.accessories, payeeId: 1);
            entity.Description = "some prior description";
            var vm = new RuleControlVM(entity);

            vm.OnRequestSave();

            Assert.Equal(RuleConditionType.MCC, entity.Condition);
            Assert.Null(entity.Description);
            Assert.Equal(Mcc.accessories, entity.MCCCategory);
        }

        [Fact]
        public void OnRequestSave_NonMCC_SetsCondition_SetsMCCCategoryToNone()
        {
            var entity = DescriptionRule("Google", payeeId: 1);
            entity.MCCCategory = Mcc.accessories; // previously had an MCC set
            var vm = new RuleControlVM(entity);

            vm.OnRequestSave();

            Assert.Equal(RuleConditionType.DescriptionContains, entity.Condition);
            Assert.Equal(Mcc.none, entity.MCCCategory);
        }

        [Fact]
        public void OnRequestSave_ReturnsEntity()
        {
            var entity = MccRule(Mcc.accessories, payeeId: 1);
            var vm = new RuleControlVM(entity);

            var result = vm.OnRequestSave();

            Assert.Same(entity, result);
        }

        // ── Entity PropertyChanged propagation ───────────────────────────────

        [Fact]
        public void EntityPropertyChanged_TriggersCanExecuteChanged()
        {
            var entity = DescriptionRule("Test", payeeId: 1);
            var vm = new RuleControlVM(entity);
            bool canExecuteChangedRaised = false;
            vm.SaveCommand.CanExecuteChanged += (_, _) => canExecuteChangedRaised = true;

            entity.PayeeId = 99;

            Assert.True(canExecuteChangedRaised);
        }
    }
}
