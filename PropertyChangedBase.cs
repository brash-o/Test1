public class PropertyChangedBase : INotifyPropertyChanged
{
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
		protected void OnPropertyChaned<TProperty>(Expression<Func<TProperty>> property)
		{
			OnPropertyChanged(property.GetMemberInfo().Name);
		}
}

public static class LinqExtension
{
  public static MemberInfo GetMemberInfo(this Expression expression)
  {
    var lambda = (LambdaExpression)expression;
    return (!(lambda.Body is UnaryExpression)?(MemberExpression)lambda.Body:(MemberExpression)((UnaryExpression)lambda.Body).Operand).Member;
  }
}
