﻿using Mniak.Network.Filters;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Mniak.Network.Wrappers
{
    public abstract class FilterableWrapper : Wrapper
    {
        public FilterableWrapper(Socket socket)
            : base(socket)
        {
        }

        List<IFilter> filters = new List<IFilter>();
        public void AddFilter(IFilter filter)
        {
            filters.Add(filter);
        }
        public void RemoveFilter(IFilter filter)
        {
            filters.Remove(filter);
        }
        public IEnumerable<IFilter> GetFilters()
        {
            return filters;
        }

        protected byte[] ProcessFiltersSend(byte[] data)
        {
            try
            {
                foreach (var filter in GetFilters())
                {
                    data = filter.FilterSend(data);
                }
                return data;
            }
            catch (FilterException fex)
            {
                FilterError(fex);
                return null;
            }
            catch (Exception ex)
            {
                FilterError(new FilterException("Unknown filter error", ex));
                return null;
            }
        }
        protected byte[] ProcessFiltersReceive(byte[] data)
        {
            try
            {
                foreach (var filter in GetFilters())
                {
                    data = filter.FilterReceive(data);
                }
                return data;
            }
            catch (FilterException fex)
            {
                FilterError(fex);
                return null;
            }
            catch (Exception ex)
            {
                FilterError(new FilterException("Unknown filter error", ex));
                return null;
            }
        }

        public delegate void FilterErrorEventHandler(FilterException filterError);
        public event FilterErrorEventHandler OnFilterError;
        private void FilterError(FilterException fex)
        {
            if (OnFilterError != null)
                OnFilterError(fex);
        }
    }
}
