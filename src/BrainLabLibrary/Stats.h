#pragma once
#include "Exports.h"
#include "EdgeValue.h"
#include <ppl.h>
#include <math.h>
#include <vector>
#include <algorithm>
#include <boost/accumulators/accumulators.hpp>
#include <boost/accumulators/statistics/stats.hpp>
#include <boost/accumulators/statistics/mean.hpp>
#include <boost/accumulators/statistics/variance.hpp>
#include <iostream>

using namespace Concurrency;
using namespace std;

namespace BrainLabNative
{
	class Stats
	{
	public:
		Stats(void);
		~Stats(void);

		void static FillVectorInc(vector<int> &vec)
		{
			struct c_unique {
				int current;
				c_unique() {current=0;}
				int operator()() {return current++;}
			} IncrementingNumber;

			generate (vec.begin(), vec.end(), IncrementingNumber);
		}

		//double static TStatByGroup(double* grp1, int szGrp1, double* grp2, int szGrp2)
		//{
		//	double m1 = 0, v1 = 0;
		//	double m2 = 0, v2 = 0;

		//	using namespace std;
		//	{
		//		using namespace boost::accumulators;
		//		using namespace boost::accumulators::extract;
		//
		//		accumulator_set<double, stats<tag::variance, tag::mean> > grp1Stats;
		//		accumulator_set<double, stats<tag::variance, tag::mean> > grp2Stats;

		//		grp1Stats = std::for_each( grp1, grp1 + szGrp1, grp1Stats );
		//		grp2Stats = std::for_each( grp2, grp2 + szGrp2, grp2Stats );
		//	
		//		m1 = mean(grp1Stats);
		//		v1 = variance(grp1Stats);
		//		
		//		m2 = mean(grp2Stats);
		//		v2 = variance(grp2Stats);

		//		// Doesn't seem to help - probably don't need
		//		//parallel_invoke(
		//		//[=, &grp1Stats, &m1, &v1] {
		//		//	grp1Stats = std::for_each( grp1, grp1 + szGrp1, grp1Stats );
		//		//	m1 = mean(grp1Stats);
		//		//	v1 = variance(grp1Stats);
		//		//},
		//		//[=, &grp2Stats, &m2, &v2] {
		//		//	grp2Stats = std::for_each( grp2, grp2 + szGrp2, grp2Stats );
		//		//	m2 = mean(grp2Stats);
		//		//	v2 = variance(grp2Stats);
		//		//});
		//	}

		//	return (m1 - m2) / sqrt( (v1 / szGrp1) + (v2 / szGrp2) );
		//}

		//double static TStatByIndex(vector<int>& idxs, double* edgeVals, int szEdgeVals, int szGrp1)
		//{
		//	double m1 = 0, v1 = 0;
		//	double m2 = 0, v2 = 0;

		//	using namespace std;
		//	{
		//		using namespace boost::accumulators;
		//		using namespace boost::accumulators::extract;
		//
		//		accumulator_set<double, stats<tag::lazy_variance> > tst;
		//		tst(1.);
		//		tst(2.);
		//		tst(3.);
		//		tst(4.);
		//		tst(5.);

		//		double val = variance(tst);

		//		accumulator_set<double, stats<tag::variance, tag::mean> > grp1Stats;
		//		accumulator_set<double, stats<tag::variance, tag::mean> > grp2Stats;

		//		for(int i=0; i<szEdgeVals; ++i)
		//		{
		//			int idx = idxs[i];

		//			if(i < szGrp1)
		//				grp1Stats(edgeVals[idx]);
		//			else
		//				grp2Stats(edgeVals[idx]);

		//			cout << edgeVals[idx] << "\t";
		//		}
		//
		//		m1 = mean(grp1Stats);
		//		v1 = variance(grp1Stats);
		//		m2 = mean(grp2Stats);
		//		v2 = variance(grp2Stats);
		//	}

		//	double tstat = (m1 - m2) / sqrt( (v1 / szGrp1) + (v2 / (szEdgeVals - szGrp1) ) );

		//	cout << tstat << "\n";

		//	return tstat;
		//}
		//
		//void static TStatByIndexEx(vector<int>& idxs, double* vals, int szVals, int szGrp1, EdgeValue& edgeValue)
		//{
		//	//int n1 = 0, n2 = 0;
		//	//double m1 = 0, m2 = 0;
		//	//double dv1 = 0, dv2 = 0;

		//	//// Loop through the vals we were passed
		//	//for (int idx = 0; idx < szVals; ++idx)
		//	//{
		//	//	double edgeVal = vals[idxs[idx]];

		//	//	//cout << edgeVal << " ";

		//	//	if (idx < szGrp1)
		//	//	{
		//	//		n1++;

		//	//		double delta = edgeVal - m1;
		//	//		m1 += delta / n1;

		//	//		if(n1 > 1)
		//	//			dv1 = dv1 + delta * (edgeVal - m1);
		//	//	}
		//	//	else
		//	//	{
		//	//		n2++;

		//	//		double delta = edgeVal - m2;
		//	//		m2 += delta / n2;

		//	//		if(n2 > 1)
		//	//			dv2 = dv2 + delta * (edgeVal - m2);
		//	//	}
		//	//}

		//	//double v1 = abs(dv1) / ( n1 - 1 );
		//	//double v2 = abs(dv2) / ( n2 - 1 );

		//	////cout << "v1: " << v1 << " v2: " << v2 << "\n";
		//	//
		//	//double tstat = 0;
		//	//if(v1 < 0.00000001 && v2 < 0.00000001)
		//	//	tstat = 0;
		//	//else
		//	//	tstat = (m1 - m2) / sqrt( ( v1 / (double)n1 ) + ( v2 / (double)n2 ) );

		//	//edgeValue.V1 = v1;
		//	//edgeValue.V2 = v2;
		//	//edgeValue.M1 = m1;
		//	//edgeValue.M2 = m2;
		//	//edgeValue.TStat = tstat;
		//}

		//BrainLabNative_API void Permute(float* edges, int edgeCount, int subjectCount, float* tstats);
	};

	typedef std::pair<int, int> Edge;

	class GraphStatsNative
	{
	public:
		vector<int> Components;

		GraphStatsNative(void);
		~GraphStatsNative(void);

		void AddEdge(int i, int j);
		int GetLargestComponentSize();
		void GetLargestComponent();
	private:
		vector<Edge> _edges;
	};
}