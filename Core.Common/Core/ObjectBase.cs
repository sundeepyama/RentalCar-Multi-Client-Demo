﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Core.Common.Utils;
using System.Collections;
using System.Runtime.Serialization;
using Core.Common.Contracts;
using FluentValidation;
using FluentValidation.Results;
using System.ComponentModel.Composition.Hosting;

namespace Core.Common.Core
{
    public abstract class ObjectBase : INotifyPropertyChanged, IDirtyCapable, IExtensibleDataObject, IDataErrorInfo
    {
        public ObjectBase()
        {
            _Validator = GetValidator();
            Validate();
        }

        protected IValidator _Validator = null;

        protected IEnumerable<ValidationFailure> _ValidationErrors = null;

        public static CompositionContainer Container { get; set; }

        List<PropertyChangedEventHandler> _PropertyChangedSubscribers = new List<PropertyChangedEventHandler>();
        private event PropertyChangedEventHandler _PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_PropertyChangedSubscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSubscribers.Add(value);
                }
            }
            remove
            {
                _PropertyChanged -= value;
                _PropertyChangedSubscribers.Remove(value);
            }
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName, true);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, true);
        }

        protected virtual void OnPropertyChanged(string propertyName, bool makeDirty)
        {
            if (_PropertyChanged != null)
                _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (makeDirty)
                _IsDirty = true;

            Validate();
        }

        #region Validator

        protected virtual IValidator GetValidator()
        {
            return null;
        }

        public IEnumerable<ValidationFailure> ValidationErrors
        {
            get { return _ValidationErrors; }
            set { }
        }

        public void Validate()
        {
            if (_Validator != null)
            {
                ValidationResult results = _Validator.Validate(this);
                _ValidationErrors = results.Errors;
            }
        }

        //[NonNavigable]
        public virtual bool IsValid
        {
            get
            {
                if (_ValidationErrors != null && _ValidationErrors.Count() > 0)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        protected bool _IsDirty;

        //[NotNavigable]
        public bool IsDirty
        {
            get { return _IsDirty; }
            set { _IsDirty = value; }
        }

        public List<ObjectBase> GetDirtyObjects()
        {
            List<ObjectBase> dirtyObjects = new List<ObjectBase>();

            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                        dirtyObjects.Add(o);

                    return false;
                }, coll => { }
                );

            return dirtyObjects;
        }

        public void CleanAll()
        {
            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                        o.IsDirty = false;

                    return false;
                }, coll => { }
                );
        }

        public virtual bool IsAnythingDirty()
        {
            bool isDirty = false;
            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                    {
                        IsDirty = true;
                        return true;
                    }
                    else
                        return false;
                }, coll => { }
                );

            return isDirty;
        }

        protected void WalkObjectGraph(Func<ObjectBase, bool> snippetForObject, Action<IList> snippetForCollection, params string[] exemptProperties)
        {
            List<ObjectBase> visited = new List<ObjectBase>();
            Action<ObjectBase> walk = null;

            List<string> exemptions = new List<string>();
            if (exemptProperties != null)
                exemptions = exemptProperties.ToList();

            walk = (o) =>
                {
                    if (o != null && !visited.Contains(o))
                    {
                        visited.Add(o);

                        bool exitWalk = snippetForObject.Invoke(o);

                        if (!exitWalk)
                        {
                            PropertyInfo[] properties = o.GetType().GetProperties();//o.GetBrowsableProperties();
                            foreach (PropertyInfo property in properties)
                            {
                                if (!exemptions.Contains(property.Name))
                                {
                                    if (property.PropertyType.IsSubclassOf(typeof(ObjectBase)))
                                    {
                                        ObjectBase obj = (ObjectBase)(property.GetValue(o, null));
                                        walk(obj);
                                    }
                                    else
                                    {
                                        IList coll = property.GetValue(o, null) as IList;
                                        if (coll != null)
                                        {
                                            snippetForCollection.Invoke(coll);

                                            foreach (object item in coll)
                                            {
                                                if (item is ObjectBase)
                                                    walk((ObjectBase)item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
        }

        public ExtensionDataObject ExtensionData { get; set; }

        public string Error
        {
            get { return String.Empty; }
        }

        public string this[string columnName]
        {
            get 
            {
                StringBuilder errors = new StringBuilder();

                if (_ValidationErrors != null && _ValidationErrors.Count() > 0)
                {
                    foreach (ValidationFailure validationError in _ValidationErrors)
                    {
                        if (validationError.PropertyName == columnName)
                            errors.AppendLine(validationError.ErrorMessage);
                    }
                }
                return errors.ToString();
            }
        }
    }
}
