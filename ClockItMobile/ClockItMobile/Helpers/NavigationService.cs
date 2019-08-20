using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms;

namespace ClockIt.Mobile.Helpers
{
	public class NavigationService : INavigationService
	{
		Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
		public NavigationPage _navigation;
        List<string> _navigatedPages=new List<string>();
        bool _willNavBack;

        public string CurrentPageKey
		{
			get
			{
				lock (_pagesByKey)
				{
					if (_navigation.CurrentPage == null)
					{
						return null;
					}

					var pageType = _navigation.CurrentPage.GetType();

					return _pagesByKey.ContainsValue(pageType) ? _pagesByKey.First(_ => _.Value == pageType).Key : null;
				}
			}
		}

		public bool GoBackBool()
        {
            if (_navigatedPages.Count>1) {
                _willNavBack = true;
                /*
                if (_navigatedPages.Last() == "AddEditSchedulePage")
                {
                    _navigatedPages.RemoveAt(_navigatedPages.Count - 1);
                    NavigateTo(_navigatedPages.Last(),App.RunningSchedule);
                }
                else
                {
                    _navigatedPages.RemoveAt(_navigatedPages.Count - 1);
                    NavigateTo(_navigatedPages.Last());
                }*/
                NavigateTo(App.Locator.SchedulesPage);
                _willNavBack = false;
                return true;
            }
            return false;
        }

		public void NavigateTo(string pageKey)
		{
			NavigateTo(pageKey, null);
		}

		public void NavigateTo(string pageKey, object parameter)
		{
			lock (_pagesByKey)
			{
				if (_pagesByKey.ContainsKey(pageKey))
				{
					var type = _pagesByKey[pageKey];
					ConstructorInfo constructor;
					object[] parameters;

					if(parameter == null)
					{
						constructor = type.GetTypeInfo()
							.DeclaredConstructors
							.FirstOrDefault(_ => !_.GetParameters().Any());

						parameters = new object[] { };
					}
					else
					{
						constructor = type.GetTypeInfo()
							.DeclaredConstructors
							.FirstOrDefault(_ =>
							{
								var p = _.GetParameters();
								return p.Count() == 1 && p[0].ParameterType == parameter.GetType();
							});

						parameters = new object[] { parameter };
					}

					if (constructor == null)
					{
						throw new InvalidOperationException($"No suitable constructor found for page {pageKey}.");
					}

					var page = constructor.Invoke(parameters) as Page;
					_navigation.Navigation.InsertPageBefore(page, _navigation.CurrentPage);
					_navigation.PopAsync();
                    if(!_willNavBack)_navigatedPages.Add(pageKey);

                }
				else
				{
					throw new ArgumentException($"No such page: {pageKey}. Did you forget to call NavigationService.Configure?", "pageKey");
				}
			}
		}

		public void Configure(string pageKey, Type pageType)
		{
			lock (_pagesByKey)
			{
				if (_pagesByKey.ContainsKey(pageKey))
				{
					_pagesByKey[pageKey] = pageType;
				}
				else
				{
					_pagesByKey.Add(pageKey, pageType);
				}
			}
		}

		public void Initialize(NavigationPage navigation)
		{
			_navigation = navigation;
            _navigatedPages.Add("MainPage");

        }

        public void GoBack()
        {
            //throw new NotImplementedException();
        }
    }
}
