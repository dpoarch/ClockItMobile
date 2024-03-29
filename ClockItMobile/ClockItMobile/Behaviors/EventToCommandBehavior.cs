﻿using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace ClockIt.Mobile.Behaviors
{
	public class EventToCommandBehavior : BehaviorBase<View>
	{
		Delegate _eventHandler;

		public static readonly BindableProperty EventNameProperty = BindableProperty.Create("EventName", typeof(string), typeof(EventToCommandBehavior), null, propertyChanged: OnEventNameChanged);
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(EventToCommandBehavior), null);
		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(EventToCommandBehavior), null);
		public static readonly BindableProperty InputConverterProperty = BindableProperty.Create("Converter", typeof(IValueConverter), typeof(EventToCommandBehavior), null);

		public string EventName
		{
			get { return (string)GetValue(EventNameProperty); }
			set { SetValue(EventNameProperty, value); }
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public IValueConverter Converter
		{
			get { return (IValueConverter)GetValue(InputConverterProperty); }
			set { SetValue(InputConverterProperty, value); }
		}

		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);
			RegisterEvent(EventName);
		}

		protected override void OnDetachingFrom(View bindable)
		{
			DeregisterEvent(EventName);
			base.OnDetachingFrom(bindable);
		}

		void RegisterEvent(string name)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				var eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
				if (eventInfo == null)
				{
					throw new ArgumentException($"EventToCommandBehavior: Cannot register the '{EventName}' event.");
				}
				var methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");
				_eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
				eventInfo.AddEventHandler(AssociatedObject, _eventHandler);
			}
		}

		void DeregisterEvent(string name)
		{
			if (!string.IsNullOrWhiteSpace(name) && _eventHandler != null)
			{
				var eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
				if (eventInfo == null)
				{
					throw new ArgumentException($"EventToCommandBehavior: Cannot unregister the '{EventName}' event.");
				}
				eventInfo.RemoveEventHandler(AssociatedObject, _eventHandler);
				_eventHandler = null;
			}
		}

		void OnEvent(object sender, object eventArgs)
		{
			if (Command != null)
			{
				object resolvedParameter;
				if (CommandParameter != null)
				{
					resolvedParameter = CommandParameter;
				}
				else if (Converter != null)
				{
					resolvedParameter = Converter.Convert(eventArgs, typeof(object), null, null);
				}
				else
				{
					resolvedParameter = eventArgs;
				}

				if (Command.CanExecute(resolvedParameter))
				{
					Command.Execute(resolvedParameter);
				}
			}
		}

		static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var behavior = (EventToCommandBehavior)bindable;

			if (behavior.AssociatedObject != null)
			{
				behavior.DeregisterEvent((string)oldValue);
				behavior.RegisterEvent((string)newValue);
			}
		}
	}
}
