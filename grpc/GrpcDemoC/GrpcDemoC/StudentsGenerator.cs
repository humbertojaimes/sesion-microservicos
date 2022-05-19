using System;
using Messages;

namespace GrpcDemoC
{
	public class StudentCatalog
	{

        public StudentCatalog()
        {
			CreateStudents();
        }

		public List<Student> Students { get; } = new();

		public void CreateStudents()
		{
            for (int i = 0; i < 10; i++)
            {
				Students.Add(new Student() {
					 Id = i,
					 FirstName = $"Name {1}",
					 LastName = $"LastName {2}"
				});
            }
		}


	}
}

