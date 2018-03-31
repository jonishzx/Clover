namespace Clover.Core.Base
{
	#region 引用的命名空间.
	

	using System;
	using System.Collections;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using Clover.Core.Common;
	using System.Collections.Generic;
	using Clover.Core.Properties;
	using Clover.I18n;
	using Clover.Core.Logging;

	
	#endregion

	

	
	
	
	public abstract class TaskBase
	{
		#region Static methods.
		

		
		
		
		
		
		
		
		public static int DispatchTasks(
			string[] rawCommands )
		{
			int count = 0;

			if ( rawCommands == null || rawCommands.Length <= 0 )
			{
				

				TaskBase[] tasks = AllTask;
				if ( tasks != null )
				{
					
					int defaultCount = 0;
					foreach ( TaskBase task in tasks )
					{
						if ( task.IsDefaultTask && task.IsActive )
						{
							defaultCount++;
						}
					}

					if ( defaultCount > 1 )
					{
                        throw new ApplicationException(
                            Strings.T(Resources.Str_Clover_Core_Base_Tasks_TaskBase_DispatchTasks_01, defaultCount));						
					}

					
					foreach ( TaskBase task in tasks )
					{
						if ( task.IsDefaultTask && task.IsActive )
						{
							task.Process( null );
							count++;
							break;
						}
					}
				}
			}
			else
			{
				

				string[] preparedRawCommands = rawCommands;

				foreach ( string rawCommand in preparedRawCommands )
				{
					if ( ExecuteTask( rawCommand ) )
					{
						count++;
					}
				}
			}

			return count;
		}

		
		
		
		
		
		
		
		
		public static bool ExecuteTask(
			string rawCommand )
		{
			TaskBase task = GetTask( rawCommand );
			if ( task != null && task.IsActive )
			{
				task.Process( task.GetCommandFromRawCommand( rawCommand ) );
				return true;
			}
			else
			{
				return false;
			}
		}

		
		
		
		
		
		public static TaskBase[] AllTask
		{
			get
			{
				List<TaskBase> list = new List<TaskBase>();

				Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
				if ( asms != null )
				{
					foreach ( Assembly asm in asms )
					{
						DoGetAllTasks( ref list, asm );
					}
				}

				Assembly a = Assembly.GetExecutingAssembly();
				DoGetAllTasks( ref list, a );

				a = Assembly.GetCallingAssembly();
				DoGetAllTasks( ref list, a );

				a = Assembly.GetEntryAssembly();
				DoGetAllTasks( ref list, a );

				if ( list.Count <= 0 )
				{
					return null;
				}
				else
				{
					return list.ToArray();
				}
			}
		}

		
		
		
		
		
		private static void DoGetAllTasks(
			ref List<TaskBase> list,
			Assembly a )
		{
			if ( a != null )
			{
				LogCentral.Current.Debug(
					string.Format(
					@"DoGetAllTasks(): Searching assembly '{0}'...",
					a.GetName().Name ) );

				Type[] types = a.GetTypes();

				foreach ( Type t in types )
				{
					if ( t.IsSubclassOf( typeof( TaskBase ) ) && !t.IsAbstract )
					{
						
						TaskBase b = Activator.CreateInstance( t ) as TaskBase;

						if ( b.IsActive )
						{
							
							bool found = false;
							foreach ( TaskBase c in list )
							{
								if ( c.GetType() == b.GetType() )
								{
									found = true;
									break;
								}
							}

							if ( !found )
							{
								list.Add( b );
							}
						}
					}
				}
			}
		}

		
		
		
		
		
		
		
		
		public static TaskBase GetTask(
			string commandSymbolicName )
		{
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			if ( asms != null )
			{
				foreach ( Assembly asm in asms )
				{
					TaskBase tba = DoGetTask( asm, commandSymbolicName );
					if ( tba != null )
					{
						return tba;
					}
				}
			}

			Assembly a = Assembly.GetExecutingAssembly();
			TaskBase tb = DoGetTask( a, commandSymbolicName );
			if ( tb != null )
			{
				return tb;
			}
			else
			{
				a = Assembly.GetCallingAssembly();
				tb = DoGetTask( a, commandSymbolicName );
				if ( tb != null )
				{
					return tb;
				}
				else
				{
					a = Assembly.GetEntryAssembly();
					tb = DoGetTask( a, commandSymbolicName );
					if ( tb != null )
					{
						return tb;
					}
					else
					{
						return null;
					}
				}
			}
		}

		
		
		
		
		
		
		private static TaskBase DoGetTask(
			Assembly a,
			string commandSymbolicName )
		{
			return DoGetTask( a, commandSymbolicName, 0 );
		}

		
		
		
		
		
		
		
		private static TaskBase DoGetTask(
			Assembly a,
			string commandSymbolicName,
			int depth )
		{
			if ( a == null || depth > 10 )
			{
				return null;
			}
			else
			{
				LogCentral.Current.Debug(
					string.Format(
					@"DoGetTask(...depth={0}): Searching assembly '{1}'...",
					depth,
					a.GetName().Name ) );

				Type[] types = a.GetTypes();

				foreach ( Type t in types )
				{
					if ( t.IsSubclassOf( typeof( TaskBase ) ) && !t.IsAbstract )
					{
						
						TaskBase b = Activator.CreateInstance( t ) as TaskBase;

						
						if ( b.Command.SymbolicName == commandSymbolicName && b.IsActive )
						{
							return b;
						}
						else
						{
							if ( b.Commands != null && b.IsActive )
							{
								foreach ( TaskCommand cn in b.Commands )
								{
									if ( cn.SymbolicName == commandSymbolicName )
									{
										return b;
									}
								}
							}
						}
					}
				}

				
				return null;
			}
		}

		
		#endregion

		#region 公共方法.
		

		
		
		
		
		public abstract void Process(
			TaskCommand command );

		
		
		
		
		
		public TaskCommand GetCommandFromRawCommand(
			string rawCommand )
		{
			if ( Commands == null )
			{
				return null;
			}
			else
			{
				foreach ( TaskCommand command in Commands )
				{
					if ( command.SymbolicName == rawCommand )
					{
						return command;
					}
				}

				return null;
			}
		}

		
		
		
		
		
		public void PumpLog(
			string text )
		{
			if ( LogCentral.Current != null )
			{
				LogCentral.Current.Info( text );
			}

			if ( GenericBase.ApplicationEnvironment != null )
			{
				GenericBase.ApplicationEnvironment.Pump( text );
			}
		}

		
		
		
		
		
		public void PumpWarning(
			string text )
		{
			if ( LogCentral.Current != null )
			{
				LogCentral.Current.Warn( text );
			}

			if ( GenericBase.ApplicationEnvironment != null )
			{
				GenericBase.ApplicationEnvironment.Pump( text );
			}
		}

		
		
		
		
		
		
		public void PumpWarning(
			string text,
			Exception x )
		{
			if ( LogCentral.Current != null )
			{
				LogCentral.Current.Warn( text, x );
			}

			if ( GenericBase.ApplicationEnvironment != null )
			{
				GenericBase.ApplicationEnvironment.Pump( text );
			}
		}

		
		
		
		
		
		public void PumpError(
			string text )
		{
			if ( LogCentral.Current != null )
			{
				LogCentral.Current.Error( text );
			}

			if ( GenericBase.ApplicationEnvironment != null )
			{
				GenericBase.ApplicationEnvironment.Pump( text );
			}
		}

		
		
		
		
		
		
		public void PumpError(
			string text,
			Exception x )
		{
			if ( LogCentral.Current != null )
			{
				LogCentral.Current.Error( text, x );
			}

			if ( GenericBase.ApplicationEnvironment != null )
			{
				GenericBase.ApplicationEnvironment.Pump( text );
			}
		}

		
		#endregion

		#region 公共属性.
		

		
		
		
		
		public abstract bool IsActive
		{
			get;
		}

		
		
		
		
		
		
		
		
		public abstract bool IsDefaultTask
		{
			get;
		}

		
		
		
		
		public abstract TaskCommand Command
		{
			get;
		}

		
		
		
		
		
		
		public abstract TaskCommand[] Commands
		{
			get;
		}

		
		
		
		
		public virtual string Description
		{
			get
			{
				return null;
			}
		}

		
		#endregion
	}

	
}