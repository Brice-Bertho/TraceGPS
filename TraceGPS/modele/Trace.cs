// Projet TraceGPS
// fichier : modele/Trace.cs
// Rôle : la classe Trace représente une trace au moyen d'une collection d'objets PointDeTrace
// Dernière mise à jour : 11/9/2018 par JM CARTRON

using System;
using System.Collections;
using System.Windows.Forms;

namespace TraceGPS
{
public class Trace
    {
        // attributs privés -----------------------------------------------------------------------------	

        private ArrayList _lesPointsDeTrace;

        // Constructeurs ------------------------------------------------------------------------------

        // Constructeur sans paramètre
        public Trace()
        {
            _lesPointsDeTrace = new ArrayList();
        }

        // Accesseurs ---------------------------------------------------------------------------------

        // Accesseur fournissant la latitude du point (en degrés décimaux)
        // retourne la latitude du point (en degrés décimaux)
        public ArrayList getLesPointsDeTrace()
        {
            return _lesPointsDeTrace;
        }

        // Méthodes ---------------------------------------------------------------------------------------

        // Fournit une chaine contenant toutes les données de l'objet
        public DateTime getDateHeureDebut()
        {
            if (_lesPointsDeTrace.Count == 0) return DateTime.MinValue;


            PointDeTrace lePremierPoint = (PointDeTrace)_lesPointsDeTrace[0];
            return lePremierPoint.getDateHeure();           
        }

        public DateTime getDateHeureFin()
        {
            if (_lesPointsDeTrace.Count == 0) return DateTime.MinValue;

            PointDeTrace leDernierPoint = (PointDeTrace)_lesPointsDeTrace[_lesPointsDeTrace.Count - 1];
            return leDernierPoint.getDateHeure();
        }
       
        public int getNombrePoints()
        {
            if (_lesPointsDeTrace.Count == 0) return 0;

            return _lesPointsDeTrace.Count;
        }

        public Point getCentre()
        {
            if (_lesPointsDeTrace.Count == 0) return null;

            int nbPoints = _lesPointsDeTrace.Count;

            PointDeTrace lePremierPoint = (PointDeTrace)_lesPointsDeTrace[0];
            double latitudeMin = lePremierPoint.getLatitude();
            double latitudeMax = lePremierPoint.getLatitude();
            double longitudeMin = lePremierPoint.getLongitude();
            double longitudeMax = lePremierPoint.getLongitude();

            for (int i = 0; i <= nbPoints-1; i++)
            {
                PointDeTrace lePoint = (PointDeTrace) _lesPointsDeTrace[i];
                if (lePoint.getLatitude() < latitudeMin )   latitudeMin = lePoint.getLatitude();
                if (lePoint.getLatitude() > latitudeMax)    latitudeMax = lePoint.getLatitude();
                if (lePoint.getLongitude() < longitudeMin)  longitudeMin = lePoint.getLongitude();           
                if (lePoint.getLongitude() > longitudeMax)   longitudeMax = lePoint.getLongitude(); 
            }

            double latitudeMoyenne = (latitudeMin + latitudeMax)/2;
            double longitudeMoyenne = (longitudeMin + longitudeMax)/2;
            Point centre = new Point(latitudeMoyenne, longitudeMoyenne, 0);
            return centre;
        }

        public double getDenivele()
        {
            if (_lesPointsDeTrace.Count == 0) return 0;

            int nbPoints = _lesPointsDeTrace.Count;

            PointDeTrace lePremierPoint = (PointDeTrace)_lesPointsDeTrace[0];
            double altitudeMin = lePremierPoint.getAltitude();
            double altitudeMax = lePremierPoint.getAltitude();

            for (int i = 0; i < nbPoints; i++)
            {
                PointDeTrace lePoint = (PointDeTrace)_lesPointsDeTrace[i];
                if (lePoint.getAltitude() < altitudeMin) altitudeMin = lePoint.getAltitude();
                if (lePoint.getAltitude() > altitudeMax) altitudeMax = lePoint.getAltitude();
            }
            double denivele = altitudeMax - altitudeMin;
            return denivele;
        }
        
        public long getDureeEnSecondes()
        {
            if (_lesPointsDeTrace.Count == 0) return 0;

            DateTime heureDebut = getDateHeureDebut();
            DateTime heureFin = getDateHeureFin();
            TimeSpan duree = heureFin - heureDebut;
            long dureeEnSeconde = (long) duree.TotalSeconds;

            return dureeEnSeconde;
        }

        public string getDureeTotale()
        {
            if (_lesPointsDeTrace.Count == 0) return "00:00:00";

            long dureeEnSeconde = getDureeEnSecondes();

            double heures = dureeEnSeconde / 3600;
            double reste = dureeEnSeconde - heures * 3600;
            double minutes = reste / 60;
            reste = reste - minutes * 60;
            double secondes = reste;
            string dureeTotale = heures.ToString("00") + ":" + minutes.ToString("00") + ":" + secondes.ToString("00");
            return dureeTotale;
        }

        public double getDistanceTotale()
        {
            if (_lesPointsDeTrace.Count == 0) return 0;

            PointDeTrace leDernierPoint = (PointDeTrace)_lesPointsDeTrace[_lesPointsDeTrace.Count - 1];

            return leDernierPoint.getDistanceCumulee();
        }
        
        public double getDenivelePositif()
        {
            if (_lesPointsDeTrace.Count == 0) return 0; 

            double denivele = 0;
            // parcours de tous les couples de points
            for (int i = 0; i < _lesPointsDeTrace.Count - 1; i += 1)
            {
                PointDeTrace lePoint1 = (PointDeTrace)_lesPointsDeTrace[i];
                PointDeTrace lePoint2 = (PointDeTrace)_lesPointsDeTrace[i + 1];
                // on teste si ça monte
                if (lePoint2.getAltitude() > lePoint1.getAltitude())
                    denivele = denivele + lePoint2.getAltitude() - lePoint1.getAltitude();
            }
            return denivele;
        }
        
        public double getDeniveleNegatif()
        {
            if (_lesPointsDeTrace.Count == 0) return 0;

            double denivele = 0;
            // parcours de tous les couples de points
            for (int i = 0; i < _lesPointsDeTrace.Count - 1; i += 1)
            {
                PointDeTrace lePoint1 = (PointDeTrace)_lesPointsDeTrace[i];
                PointDeTrace lePoint2 = (PointDeTrace)_lesPointsDeTrace[i + 1];
                // on teste si ça descend
                if (lePoint2.getAltitude() < lePoint1.getAltitude())
                    denivele = denivele + lePoint1.getAltitude() - lePoint2.getAltitude();
            }
            return denivele;
        }

        public double getVitesseMoyenne()
        {
            if (_lesPointsDeTrace.Count == 0) return 0;

            double distanceTotale = getDistanceTotale();
            double duree = getDureeEnSecondes();
            double dureeEnHeure = duree / 3600;

            double vitesseMoy = distanceTotale / dureeEnHeure;

            return vitesseMoy;
        }

        public void ajouterPoint(PointDeTrace nouveauPoint)
        {
            if (_lesPointsDeTrace.Count == 0)
            {
                nouveauPoint.setTempsCumule(0);
                nouveauPoint.setDistanceCumulee(0);
                nouveauPoint.setVitesse(0);
            }
            else
            {
                PointDeTrace dernierPoint = (PointDeTrace) _lesPointsDeTrace[_lesPointsDeTrace.Count - 1];

                double distance = Point.getDistance(dernierPoint, nouveauPoint);
                nouveauPoint.setDistanceCumulee(dernierPoint.getDistanceCumulee() + distance);

                TimeSpan duree = nouveauPoint.getDateHeure() - dernierPoint.getDateHeure();
                double temps = duree.TotalSeconds;

                nouveauPoint.setTempsCumule(dernierPoint.getTempsCumule() + (long)temps);
                nouveauPoint.setVitesse(distance / (temps / 3600));
            }

            _lesPointsDeTrace.Add(nouveauPoint);
        }

        public void viderListePoints()
        {
            _lesPointsDeTrace.Clear();
        }

        // Fournit une chaine contenant toutes les données de l'objet
        public String toString()
        {
            if (_lesPointsDeTrace.Count == 0) return "Nombre de points :\t\t" + "00000" + "\n";

            String msg = "";
            msg += "Nombre de points :\t\t" + getNombrePoints().ToString("00000") + "\n";
            if (getNombrePoints() > 0)
            {
                msg += "Heure de début :\t\t" + getDateHeureDebut().ToString("dd/MM/yyyy HH:mm:ss") + "\n";
                msg += "Heure de fin :\t\t" + getDateHeureFin().ToString("dd/MM/yyyy HH:mm:ss") + "\n";
                msg += "Durée totale :\t\t" + getDureeTotale() + "\n";
                msg += "Distance totale en Km :\t" + getDistanceTotale().ToString("000.00") + "\n";
                msg += "Dénivelé en m :\t\t" + getDenivele().ToString("0000.00") + "\n";
                msg += "Dénivelé positif en m :\t" + getDenivelePositif().ToString("0000.00") + "\n";
                msg += "Dénivelé négatif en m :\t" + getDeniveleNegatif().ToString("0000.00") + "\n";
                msg += "Vitesse moyenne en Km/h :\t" + getVitesseMoyenne().ToString("00.00") + "\n";
                msg += "Centre du parcours :\n";
                msg += "   - Latitude :\t\t" + getCentre().getLatitude().ToString("000.000") + "\n";
                msg += "   - Longitude :\t\t" + getCentre().getLongitude().ToString("000.000") + "\n";
                msg += "   - Altitude :\t\t" + getCentre().getAltitude().ToString("000.000") + "\n";
            }
            return msg;
        }

    } // fin de la classe
} // fin du namespace
