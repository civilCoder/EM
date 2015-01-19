using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;


namespace fixGradeTagLinks
{
    public class HighlightDrawableOverrule : DrawableOverrule
    {
        private int _foreColorIndex;
        private int _backColorIndex;
        private bool _origialOverruling = false;
        private List<ObjectId> _highlightEnts;
        private bool _started = false;

        public HighlightDrawableOverrule (int foreColor, int backColor)
        {
            _foreColorIndex = foreColor;
            _backColorIndex = backColor;
            _highlightEnts = new List<ObjectId>();
        }

        public bool Started
        {
            get { return _started; }
        }

        public ObjectId[] SelectedEntIds
        {
            get { return _highlightEnts.ToArray(); }
        }

        public HighlightDrawableOverrule (int foreColor, int backColor, ObjectId[] entIds) : this(foreColor, backColor)
        {
            _highlightEnts.AddRange(entIds);
        }

        public void Start ()
        {
            _origialOverruling = DrawableOverrule.Overruling;

            DrawableOverrule.AddOverrule(RXClass.GetClass(typeof(Entity)), this, true);
            DrawableOverrule.Overruling = true;
            _started = true;

            Regen();
        }

        public void Stop ()
        {
            DrawableOverrule.RemoveOverrule(RXClass.GetClass(typeof(Entity)), this);
            DrawableOverrule.Overruling = _origialOverruling;
            _started = false;

            Regen();
        }

        public void AddHightlightEntities (ObjectId[] entIds)
        {
            bool added = false;
            foreach (var id in entIds)
            {
                if (!_highlightEnts.Contains(id))
                {
                    _highlightEnts.Add(id);
                    if (!added) added = true;
                }
            }

            if (_started && added) Regen();
        }

        public void RemoveHighlightEntities (ObjectId[] entIds)
        {
            bool removed = false;
            foreach (var id in entIds)
            {
                if (!_highlightEnts.Contains(id))
                {
                    _highlightEnts.Remove(id);
                    if (!removed) removed = true;
                }
            }

            if (_started && removed) Regen();
        }

        public void ClearHightlightEntities ()
        {
            _highlightEnts.Clear();
            if (_started) Regen();
        }

        public override bool WorldDraw (Drawable drawable, WorldDraw wd)
        {
            wd.SubEntityTraits.Color = (short)_backColorIndex;

            if (_highlightEnts.Count > 0)
            {
                Entity ent = drawable as Entity;
                if (ent != null)
                {
                    if (_highlightEnts.Contains(ent.ObjectId))
                        wd.SubEntityTraits.Color = (short)_foreColorIndex;
                }
            }

            return base.WorldDraw(drawable, wd);
        }

        private void Regen ()
        {
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
        }

    }
}
