using NUnit.Framework;

namespace Tests.UI
{
    public class DungeonLabUISizeTests : DungeonLabUITestBase
    {
        private const int MinSize = 5;
        private const int MaxSize = 51;
        private const int DefaultWidth = 21;
        private const int DefaultHeight = 21;
        
        protected override void SetUpTest()
        {
            SetPrivateField("minSize", MinSize);
            SetPrivateField("maxSize", MaxSize);
            SetPrivateField("defaultWidth", DefaultWidth);
            SetPrivateField("defaultHeight", DefaultHeight);
        }

        [Test]
        public void OnWidthDecreaseClicked_DecreasesWidth()
        {
            widthInput.text = "10";

            dungeonLabUI.OnWidthDecreaseClicked();

            Assert.AreEqual("9", widthInput.text);
        }

        [Test]
        public void OnWidthDecreaseClicked_AtMinimum_KeepsMinimumWidth()
        {
            widthInput.text = MinSize.ToString();

            dungeonLabUI.OnWidthDecreaseClicked();

            Assert.AreEqual(MinSize.ToString(), widthInput.text);
            Assert.IsFalse(widthDecreaseButton.interactable);
            Assert.IsTrue(widthIncreaseButton.interactable);
        }

        [Test]
        public void OnWidthIncreaseClicked_IncreasesWidth()
        {
            widthInput.text = "10";

            dungeonLabUI.OnWidthIncreaseClicked();

            Assert.AreEqual("11", widthInput.text);
        }

        [Test]
        public void OnWidthIncreaseClicked_AtMaximum_KeepsMaximumWidth()
        {
            widthInput.text = MaxSize.ToString();

            dungeonLabUI.OnWidthIncreaseClicked();

            Assert.AreEqual(MaxSize.ToString(), widthInput.text);
            Assert.IsTrue(widthDecreaseButton.interactable);
            Assert.IsFalse(widthIncreaseButton.interactable);
        }

        [Test]
        public void OnHeightDecreaseClicked_DecreasesHeight()
        {
            heightInput.text = "10";

            dungeonLabUI.OnHeightDecreaseClicked();

            Assert.AreEqual("9", heightInput.text);
        }

        [Test]
        public void OnHeightDecreaseClicked_AtMinimum_KeepsMinimumHeight()
        {
            heightInput.text = MinSize.ToString();

            dungeonLabUI.OnHeightDecreaseClicked();

            Assert.AreEqual(MinSize.ToString(), heightInput.text);
            Assert.IsFalse(heightDecreaseButton.interactable);
            Assert.IsTrue(heightIncreaseButton.interactable);
        }

        [Test]
        public void OnHeightIncreaseClicked_IncreasesHeight()
        {
            heightInput.text = "10";

            dungeonLabUI.OnHeightIncreaseClicked();

            Assert.AreEqual("11", heightInput.text);
        }

        [Test]
        public void OnHeightIncreaseClicked_AtMaximum_KeepsMaximumHeight()
        {
            heightInput.text = MaxSize.ToString();

            dungeonLabUI.OnHeightIncreaseClicked();

            Assert.AreEqual(MaxSize.ToString(), heightInput.text);
            Assert.IsTrue(heightDecreaseButton.interactable);
            Assert.IsFalse(heightIncreaseButton.interactable);
        }
        
        [Test]
        public void OnWidthEndEdit_WithInvalidInput_UsesDefaultWidth()
        {
            widthInput.text = "abc";

            dungeonLabUI.OnWidthEndEdit();

            Assert.AreEqual(DefaultWidth.ToString(), widthInput.text);
            Assert.IsTrue(widthDecreaseButton.interactable);
            Assert.IsTrue(widthIncreaseButton.interactable);
        }

        [Test]
        public void OnWidthEndEdit_BelowMinimum_ClampsToMinimum()
        {
            widthInput.text = (MinSize - 1).ToString();

            dungeonLabUI.OnWidthEndEdit();

            Assert.AreEqual(MinSize.ToString(), widthInput.text);
            Assert.IsFalse(widthDecreaseButton.interactable);
            Assert.IsTrue(widthIncreaseButton.interactable);
        }

        [Test]
        public void OnWidthEndEdit_AboveMaximum_ClampsToMaximum()
        {
            widthInput.text = (MaxSize + 1).ToString();

            dungeonLabUI.OnWidthEndEdit();

            Assert.AreEqual(MaxSize.ToString(), widthInput.text);
            Assert.IsTrue(widthDecreaseButton.interactable);
            Assert.IsFalse(widthIncreaseButton.interactable);
        }

        [Test]
        public void OnHeightEndEdit_WithInvalidInput_UsesDefaultHeight()
        {
            heightInput.text = "abc";

            dungeonLabUI.OnHeightEndEdit();

            Assert.AreEqual(DefaultHeight.ToString(), heightInput.text);
            Assert.IsTrue(heightDecreaseButton.interactable);
            Assert.IsTrue(heightIncreaseButton.interactable);
        }

        [Test]
        public void OnHeightEndEdit_BelowMinimum_ClampsToMinimum()
        {
            heightInput.text = (MinSize - 1).ToString();

            dungeonLabUI.OnHeightEndEdit();

            Assert.AreEqual(MinSize.ToString(), heightInput.text);
            Assert.IsFalse(heightDecreaseButton.interactable);
            Assert.IsTrue(heightIncreaseButton.interactable);
        }

        [Test]
        public void OnHeightEndEdit_AboveMaximum_ClampsToMaximum()
        {
            heightInput.text = (MaxSize + 1).ToString();

            dungeonLabUI.OnHeightEndEdit();

            Assert.AreEqual(MaxSize.ToString(), heightInput.text);
            Assert.IsTrue(heightDecreaseButton.interactable);
            Assert.IsFalse(heightIncreaseButton.interactable);
        }
    }
}