using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Eto
{
	/// <summary>
	/// The default style provider which supports setting styles from delegates.
	/// </summary>
    public class DefaultStyleProvider : IStyleProvider
    {
        readonly Dictionary<object, IList<Action<object>>> styleMap = new Dictionary<object, IList<Action<object>>>();
        readonly Dictionary<object, IList<Action<object>>> cascadingStyleMap = new Dictionary<object, IList<Action<object>>>();

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:DefaultStyleProvider"/> allows inheriting styles from the parent object.
		/// </summary>
		/// <remarks>
		/// This is used when applying cascading styles, sometimes the user or the provider will not want
		/// parent styles to be applicable.
		/// </remarks>
		/// <value><c>true</c> to inherit parent styles; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
        public bool Inherit { get; set; } = true;

		#region Events

		/// <summary>
		/// Event to handle when a widget is being styled
		/// </summary>
		public event Action<object> StyleWidget;

        #endregion

        /// <summary>
        /// Adds a style for a widget
        /// </summary>
        /// <remarks>
        /// Styling a widget allows you to access the widget or its native handler class and apply logic.
        /// 
        /// Styling a handler allows you to access both the platform-specifics for the widget.  
        /// It requires you to add a reference to one of the Eto.*.dll's so that you can utilize
        /// the platform handler directly.  Typically this would be called before your application is run.
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// // make all labels vertically aligned
        /// style.Add<Label>(null, label => {
        /// 	label.VerticalAlignment = VerticalAlignment.Center;
        /// 	// other stuff
        /// });
        /// 
        /// // access native controls to modify things that Eto doesn't give direct access to
        /// style.Add<Eto.Mac.Forms.Controls.ButtonHandler>(null, handler => {
        /// 	handler.Control.SomeProperty = someValue;
        /// 	// other stuff
        /// });
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="T">Type of the widget or handler to style</typeparam>
        /// <param name="style">Identifier of the style</param>
        /// <param name="handler">Delegate with your logic to style the widget</param>
        public void Add<T>(string style, Action<T> handler)
            where T : class
        {
            var list = CreateStyleList((object)style ?? typeof(T));
            list.Add(widget =>
            {
				if (widget is T control)
					handler(control);
				else if (widget is IHandlerSource handlerSource && handlerSource.Handler is T handlerControl)
					handler(handlerControl);
            });
            cascadingStyleMap.Clear();
        }

		/// <summary>
		/// Clears all styles from this provider
		/// </summary>
		public void Clear()
		{
			styleMap.Clear();
			cascadingStyleMap.Clear();
		}

		IList<Action<object>> CreateStyleList(object style)
        {
			if (!styleMap.TryGetValue(style, out var styleHandlers))
			{
				styleHandlers = new List<Action<object>>();
				styleMap[style] = styleHandlers;
			}
			return styleHandlers;
        }

        IList<Action<object>> GetStyleList(object style)
        {
            return styleMap.TryGetValue(style, out var styleHandlers) ? styleHandlers : null;
        }

        IList<Action<object>> GetCascadingStyleList(Type type)
        {
            if (type == null)
                return null;
            // get a cached list of cascading styles so we don't have to traverse each time
            if (cascadingStyleMap.TryGetValue(type, out var childHandlers))
            {
                return childHandlers;
            }

            // don't have a cascading style set, so build one
            // styles are applied in order from superclass styles down to subclass styles.
            IEnumerable<Action<object>> styleHandlers = Enumerable.Empty<Action<object>>();
            Type currentType = type;
            do
            {
                if (styleMap.TryGetValue(currentType, out var typeStyles) && typeStyles != null)
                    styleHandlers = typeStyles.Concat(styleHandlers);
            }
            while ((currentType = currentType.GetBaseType()) != null);

            // create a cached list, but if its empty don't store it
            childHandlers = styleHandlers.ToList();
            if (childHandlers.Count == 0)
                childHandlers = null;
            cascadingStyleMap.Add(type, childHandlers);

            return childHandlers;
        }

        void ApplyStyles(object widget, string style)
        {
            if (widget != null && !string.IsNullOrEmpty(style))
            {
                var styles = style.Split(' ');
                for (int i = 0; i < styles.Length; i++)
                {
                    var currentStyle = styles[i];
                    var styleHandlers = GetStyleList(currentStyle);
                    if (styleHandlers != null)
                    {
                        for (int j = 0; j < styleHandlers.Count; j++)
                        {
                            var styleHandler = styleHandlers[j];
                            styleHandler(widget);
                        }
                    }
                }
            }
            StyleWidget?.Invoke(widget);
        }

        void ApplyDefaults(object widget)
        {
			if (widget == null)
				return;

			var styles = GetCascadingStyleList(widget.GetType());
			if (styles != null)
			{

				for (int i = 0; i < styles.Count; i++)
				{
					var styleHandler = styles[i];
					styleHandler(widget);
				}
			}
		}

		bool IStyleProvider.Inherit => Inherit;

		void IStyleProvider.ApplyCascadingStyle(object container, object widget, string style)
		{
			ApplyDefaults(widget);
			if (widget is IHandlerSource handlerSource)
				ApplyDefaults(handlerSource.Handler);
			ApplyStyles(widget, style);
		}

        void IStyleProvider.ApplyDefault(object widget) => ApplyDefaults(widget);

		void IStyleProvider.ApplyStyle(object widget, string style) => ApplyStyles(widget, style);
	}
}

